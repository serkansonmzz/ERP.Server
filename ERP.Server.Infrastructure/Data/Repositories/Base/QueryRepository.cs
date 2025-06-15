using System.Linq.Expressions;
using ERP.Server.Domain.Entities.Common;
using ERP.Server.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace ERP.Server.Infrastructure.Data.Repositories.Base;

public abstract class QueryRepository<T> : IQueryRepository<T> where T : Entity
{
    protected readonly IMongoCollection<T> _collection;

    protected QueryRepository(MongoDbContext context, string collectionName)
    {
        _collection = context.Database.GetCollection<T>(collectionName);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.Empty;
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await _collection.Find(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate, 
        CancellationToken cancellationToken = default)
    {
        return await _collection.CountDocumentsAsync(predicate, cancellationToken: cancellationToken) > 0;
    }

    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null, 
        CancellationToken cancellationToken = default)
    {
        return predicate == null
            ? (int)await _collection.CountDocumentsAsync(FilterDefinition<T>.Empty, cancellationToken: cancellationToken)
            : (int)await _collection.CountDocumentsAsync(predicate, cancellationToken: cancellationToken);
    }
}
