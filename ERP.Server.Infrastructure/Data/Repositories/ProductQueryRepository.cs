using System.Linq.Expressions;
using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums;
using ERP.Server.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Server.Infrastructure.Data.Repositories;

public class ProductQueryRepository : Repository<Product>, IProductQueryRepository
{
    public ProductQueryRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Product>> GetProductsByTypeAsync(ProductType type, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Type == type)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync(cancellationToken);

        var searchTermLower = searchTerm.ToLower();
        return await _dbSet
            .Where(p => p.Name.ToLower().Contains(searchTermLower) ||
                       (p.Description != null && p.Description.ToLower().Contains(searchTermLower)))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .OrderBy(p => p.Price)
            .ToListAsync(cancellationToken);
    }
}
