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

    public async Task UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await _dbSet.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        // product.StockQuantity = quantity; // Uncomment when StockQuantity is added to Product
        product.SetUpdated();
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePriceAsync(Guid productId, decimal newPrice, CancellationToken cancellationToken = default)
    {
        var product = await _dbSet.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        // product.Price = newPrice; // Uncomment when Price is added to Product
        product.SetUpdated();
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}
