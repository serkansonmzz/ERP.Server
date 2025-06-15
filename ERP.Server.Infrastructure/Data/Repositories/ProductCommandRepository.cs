using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums;
using ERP.Server.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Server.Infrastructure.Data.Repositories;

public class ProductCommandRepository : Base.EfCommandRepository<Product>, IProductCommandRepository
{
    public ProductCommandRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }

    public async Task UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await _dbSet.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        product.UpdateStock(quantity);
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePriceAsync(Guid productId, decimal newPrice, CancellationToken cancellationToken = default)
    {
        var product = await _dbSet.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        product.UpdatePrice(newPrice);
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}
