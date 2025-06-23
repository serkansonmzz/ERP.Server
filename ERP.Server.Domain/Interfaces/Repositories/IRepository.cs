using System.Linq.Expressions;
using ERP.Server.Domain.Entities.Common;

namespace ERP.Server.Domain.Interfaces.Repositories;

// Base Command Repository Interface
public interface ICommandRepository<T> where T : Entity
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

// Base Query Repository Interface
public interface IQueryRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

// Backward compatibility - Eski kodu kırmamak için bırakıyoruz, yeni kodlarda kullanmayın
[Obsolete("Use ICommandRepository and IQueryRepository instead for better CQRS separation")]
public interface IRepository<T> : ICommandRepository<T>, IQueryRepository<T> where T : Entity
{
}

