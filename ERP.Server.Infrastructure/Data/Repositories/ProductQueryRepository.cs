using System.Linq.Expressions;
using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Common;
using ERP.Server.Domain.Entities.Enums;
using ERP.Server.Domain.Interfaces.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ERP.Server.Infrastructure.Data.Repositories;

public class ProductQueryRepository : Base.QueryRepository<Product>, IProductQueryRepository
{
    public ProductQueryRepository(MongoDbContext context) 
        : base(context, "products")
    {
        // Create indexes for better query performance
        CreateIndexes();
    }
    
    private void CreateIndexes()
    {
        // Create index for frequently queried fields
        var indexKeysDefinition = Builders<Product>.IndexKeys
            .Ascending(p => p.IsDeleted)
            .Ascending(p => p.Type)
            .Ascending(p => p.IsActive)
            .Ascending(p => p.Price);
            
        var indexOptions = new CreateIndexOptions { Background = true };
        var model = new CreateIndexModel<Product>(indexKeysDefinition, indexOptions);
        
        // Apply the index
        _collection.Indexes.CreateOne(model);
    }

    public async Task<IReadOnlyList<Product>> GetProductsByTypeAsync(ProductType type, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.Type, type),
            Builders<Product>.Filter.Eq(p => p.IsDeleted, false)
        );
        
        return await _collection.Find(filter)
            .SortBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAsync(p => !p.IsDeleted, cancellationToken);

        var searchTermLower = searchTerm.ToLower();
        
        // Create a filter that combines search term with soft delete check
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.IsDeleted, false),
            Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<Product>.Filter.Regex(p => p.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<Product>.Filter.Regex(p => p.Sku, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<Product>.Filter.Regex(p => p.Barcode, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            )
        );

        // Use text search if you've created a text index
        // var sort = Builders<Product>.Sort.MetaTextScore("textScore");
        
        return await _collection.Find(filter)
            // .Sort(sort)
            .Limit(100) // Limit results for performance
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.IsDeleted, false),
            Builders<Product>.Filter.Gte(p => p.Price, minPrice),
            Builders<Product>.Filter.Lte(p => p.Price, maxPrice),
            Builders<Product>.Filter.Eq(p => p.IsActive, true)
        );

        var sort = Builders<Product>.Sort.Ascending(p => p.Price);
        
        return await _collection
            .Find(filter)
            .Sort(sort)
            .Limit(1000) // Limit results for performance
            .ToListAsync(cancellationToken);
    }
}
