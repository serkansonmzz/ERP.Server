using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums;
using ERP.Server.Domain.Interfaces.Repositories;
using ERP.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ERP.Server.Infrastructure.Services.Outbox;

public class OutboxProcessorBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessorBackgroundService> _logger;

    public OutboxProcessorBackgroundService(IServiceScopeFactory scopeFactory, ILogger<OutboxProcessorBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
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

        var pendingMessages = await outboxRepository.GetAllAsync(cancellationToken);
        if (pendingMessages.Count == 0)
            return;

        foreach (var message in pendingMessages)
        {
            try
            {
                if (message.TableName == TableNameEnum.Product)
                {
                    var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == message.RecordId, cancellationToken);
                    if (product != null)
                    {
                        var productsCollection = mongoContext.Database.GetCollection<Product>("products");
                        switch (message.Operation)
                        {
                            case OperationEnum.Insert:
                            case OperationEnum.Update:
                                await productsCollection.ReplaceOneAsync(
                                    Builders<Product>.Filter.Eq(p => p.Id, product.Id),
                                    product,
                                    new ReplaceOptions { IsUpsert = true },
                                    cancellationToken);
                                break;
                            case OperationEnum.Delete:
                                await productsCollection.DeleteOneAsync(
                                    Builders<Product>.Filter.Eq(p => p.Id, product.Id),
                                    cancellationToken);
                                break;
                        }
                        message.IsCompleted = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing outbox message {OutboxId}", message.Id);
                 message.TryCount += 1;
            }

            await outboxRepository.UpdateAsync(message, cancellationToken);
            await outboxRepository.SaveChangesAsync(cancellationToken);
            
        }
    }
}
