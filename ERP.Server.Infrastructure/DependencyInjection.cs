using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using ERP.Server.Infrastructure.Data;
using ERP.Server.Infrastructure.Data.Repositories;
using ERP.Server.Domain.Interfaces.Repositories;
using ERP.Server.Domain.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace ERP.Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MSSQL DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));

        // MongoDB Configuration
        services.Configure<MongoDbSettings>(
            configuration.GetSection("MongoDbSettings"));
                
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });
            
        services.AddSingleton<MongoDbContext>();

        // Register Command Repositories (MSSQL)
        services.AddScoped<IProductCommandRepository, ProductCommandRepository>();
        
        // Register Query Repositories (MongoDB)
        services.AddScoped<IProductQueryRepository, ProductQueryRepository>();
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork>(sp =>
        {
            var dbContext = sp.GetService<ApplicationDbContext>();
            var mongoContext = sp.GetService<MongoDbContext>();
            var productCommandRepository = sp.GetService<IProductCommandRepository>();
            var productQueryRepository = sp.GetService<IProductQueryRepository>();
            return new UnitOfWork(dbContext, mongoContext, productCommandRepository, productQueryRepository);
        });
        
        return services;
    }
}

/// <summary>
/// Unit of Work implementation that coordinates work between MSSQL (Commands) and MongoDB (Queries)
/// </summary>
public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly MongoDbContext _mongoContext;
    private bool _disposed;
    private IProductCommandRepository? _productCommandRepository;
    private IProductQueryRepository? _productQueryRepository;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(
        ApplicationDbContext dbContext,
        MongoDbContext mongoContext,
        IProductCommandRepository productCommandRepository,
        IProductQueryRepository productQueryRepository)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mongoContext = mongoContext ?? throw new ArgumentNullException(nameof(mongoContext));
        _productCommandRepository = productCommandRepository ?? throw new ArgumentNullException(nameof(productCommandRepository));
        _productQueryRepository = productQueryRepository ?? throw new ArgumentNullException(nameof(productQueryRepository));
    }

    public IProductCommandRepository ProductCommandRepository => _productCommandRepository!;
    public IProductQueryRepository ProductQueryRepository => _productQueryRepository!;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
