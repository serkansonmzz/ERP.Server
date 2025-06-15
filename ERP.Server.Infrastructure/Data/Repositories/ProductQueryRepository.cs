using System.Linq.Expressions;
using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums;
using ERP.Server.Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace ERP.Server.Infrastructure.Data.Repositories;

public class ProductQueryRepository : Base.QueryRepository<Product>, IProductQueryRepository
{
    public ProductQueryRepository(MongoDbContext context) 
        : base(context, "products")
    {
    }

    public async Task<IReadOnlyList<Product>> GetProductsByTypeAsync(ProductType type, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Type, type);
        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync(cancellationToken);

        var searchTermLower = searchTerm.ToLower();
        var filter = Builders<Product>.Filter.Or(
            Builders<Product>.Filter.Where(p => p.Name.ToLower().Contains(searchTermLower)),
            Builders<Product>.Filter.Where(p => p.Description != null && p.Description.ToLower().Contains(searchTermLower))
        );

        return await _collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Gte("Price", minPrice),
            Builders<Product>.Filter.Lte("Price", maxPrice)
        );

        var sort = Builders<Product>.Sort.Ascending("Price");
        
        return await _collection
            .Find(filter)
            .Sort(sort)
            .ToListAsync(cancellationToken);
    }
}
