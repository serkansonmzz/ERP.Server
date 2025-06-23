using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums;
using ERP.Server.Domain.Interfaces.Repositories;
using ERP.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver; // Added for Builders<T>

namespace ERP.Server.Infrastructure.Services.Outbox;

public class OutboxProcessorBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessorBackgroundService> _logger;
    private const int MaxRetryCount = 5; // Max deneme sayısı

    public OutboxProcessorBackgroundService(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessorBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private async Task ProcessOutboxAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var mongoContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

        // Sadece işlenmemiş ve deneme limitini aşmamış mesajları al
        var pendingMessages = await outboxRepository.GetByConditionAsync(
            filter: m => !m.IsCompleted && m.TryCount < MaxRetryCount,
            cancellationToken: cancellationToken);

        if (!pendingMessages.Any())
        {
            return;
        }

        _logger.LogInformation("{Count} pending messages found. Processing...", pendingMessages.Count);

        foreach (var message in pendingMessages)
        {
            try
            {
                if (message.TableName == TableNameEnum.Product)
                {
                    var product = await dbContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == message.RecordId, cancellationToken);
                    if (product != null)
                    {
                        var productsCollection = mongoContext.Database.GetCollection<Product>("products");
                        switch (message.Operation)
                        {
                            case OperationEnum.Insert:
                            case OperationEnum.Update:
                                await productsCollection.ReplaceOneAsync(p => p.Id == product.Id, product, new ReplaceOptions { IsUpsert = true }, cancellationToken);
                                _logger.LogInformation("Product {ProductId} inserted/updated in MongoDB.", product.Id);
                                break;
                            case OperationEnum.Delete:
                                await productsCollection.DeleteOneAsync(Builders<Product>.Filter.Eq(p => p.Id, message.RecordId), cancellationToken);
                                _logger.LogInformation("Product {ProductId} deleted from MongoDB.", message.RecordId);
                                break;
                        }
                        message.IsCompleted = true;
                    }
                    else
                    {
                        _logger.LogWarning("Product with Id {RecordId} not found for outbox message {OutboxId}. It might have been deleted.", message.RecordId, message.Id);
                        message.IsCompleted = true;
                    }
                }
            }
            catch (Exception ex)
            {                
                _logger.LogError(ex, "Error processing outbox message {OutboxId}. Attempt {TryCount}", message.Id, message.TryCount + 1);
                message.TryCount++; // Sadece hata durumunda deneme sayısını artır.
            }

            if (message.TryCount >= MaxRetryCount)
            {
                _logger.LogWarning("Outbox message {OutboxId} has reached max retries and will be marked as failed.", message.Id);
                message.IsCompleted = true; 
            }

            await outboxRepository.UpdateAsync(message, cancellationToken);
        }

        await outboxRepository.SaveChangesAsync(cancellationToken);
    }
}
