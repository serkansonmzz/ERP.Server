using ERP.Server.Domain.Interfaces.Repositories;

namespace ERP.Server.Domain.Interfaces;

/// <summary>
/// Defines the interface for the Unit of Work pattern to coordinate transactions across repositories
/// </summary>
public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Gets the product command repository for write operations
    /// </summary>
    IProductCommandRepository ProductCommandRepository { get; }

    /// <summary>
    /// Gets the product query repository for read operations
    /// </summary>
    IProductQueryRepository ProductQueryRepository { get; }

    /// <summary>
    /// Begins a new transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in this unit of work
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
