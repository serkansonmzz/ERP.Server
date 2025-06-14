using ERP.Server.Domain.Interfaces.Repositories;

namespace ERP.Server.Domain.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    // Repository'ler burada tanımlanacak
    IProductRepository ProductRepository { get; }
    
    // Transaction yönetimi için
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    
    // Değişiklikleri kaydetmek için
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
