using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using ERP.Server.Infrastructure.Data;
using ERP.Server.Infrastructure.Data.Repositories;
using ERP.Server.Domain.Interfaces.Repositories;
using ERP.Server.Domain.Entities;

namespace ERP.Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MSSQL Configuration
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // MongoDB Configuration
        services.Configure<MongoDbSettings>(
            configuration.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<MongoDbContext>();

        // Register repositories
        services.AddScoped<IProductCommandRepository, ProductCommandRepository>();
        services.AddScoped<IProductQueryRepository, ProductQueryRepository>();
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}

// Temporary Unit of Work implementation for MongoDB
public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly MongoDbContext _context;
    private IProductCommandRepository? _productCommandRepository;
    private IProductQueryRepository? _productQueryRepository;

    public UnitOfWork(MongoDbContext context)
    {
        _context = context;
    }

    public IProductCommandRepository ProductCommandRepository => 
        _productCommandRepository ??= new ProductCommandRepository(_context);
        
    public IProductQueryRepository ProductQueryRepository => 
        _productQueryRepository ??= new ProductQueryRepository(_context);

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // MongoDB doesn't support transactions in the same way as relational databases
        // For multi-document transactions, you would need to use a MongoDB session
        return Task.CompletedTask;
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        // In MongoDB, operations are atomic by default
        return Task.CompletedTask;
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        // Rollback is not directly supported in the same way as in relational databases
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // In MongoDB, changes are saved immediately
        return 1;
    }

    public void Dispose()
    {
        // Cleanup if needed
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
