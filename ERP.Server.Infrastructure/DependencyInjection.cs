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
using ERP.Server.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

using ERP.Server.Application.Interfaces;
using ERP.Server.Infrastructure.Services.Auth;

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
        var mongoDbSettings = new MongoDbSettings();
        configuration.GetSection("MongoDbSettings").Bind(mongoDbSettings);
        
        services.AddSingleton(mongoDbSettings);
        
        services.AddSingleton<IMongoClient>(sp =>
        {
            return new MongoClient(mongoDbSettings.ConnectionString);
        });
            
        services.AddSingleton<MongoDbContext>();

        // Register Command Repositories (MSSQL)
        services.AddScoped<IProductCommandRepository, ProductCommandRepository>();
        
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // Register Query Repositories (MongoDB)
        services.AddScoped<IProductQueryRepository, ProductQueryRepository>();
        
        // Register Unit of Work with null checks
        services.AddScoped<IUnitOfWork>(sp =>
        {
            var dbContext = sp.GetRequiredService<ApplicationDbContext>();
            var mongoContext = sp.GetRequiredService<MongoDbContext>();
            var productCommandRepository = sp.GetRequiredService<IProductCommandRepository>();
            var productQueryRepository = sp.GetRequiredService<IProductQueryRepository>();
            
            if (dbContext == null || mongoContext == null || 
                productCommandRepository == null || productQueryRepository == null)
            {
                throw new InvalidOperationException("Required services for UnitOfWork are not registered");
            }
            
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

    public IProductCommandRepository Products => _productCommandRepository!;
    public IProductQueryRepository ProductQueries => _productQueryRepository!;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
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
            await RollbackTransactionAsync(cancellationToken);
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

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
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

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _currentTransaction?.Dispose();
                _dbContext.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.DisposeAsync().ConfigureAwait(false);
            _currentTransaction = null;
        }
        await _dbContext.DisposeAsync().ConfigureAwait(false);
    }
}
