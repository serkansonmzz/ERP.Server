using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums;
using ERP.Server.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ERP.Server.Infrastructure.Data.Repositories;

public class ProductCommandRepository : Repository<Product>, IProductCommandRepository
{
    public ProductCommandRepository(DbContext dbContext) : base(dbContext)
    {
    }

    public async Task UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(productId, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        // Burada stok güncelleme mantığı uygulanabilir
        // Örnek: product.StockQuantity = quantity;
        // Gerçek uygulamada stok takibi için daha detaylı bir yapı gerekebilir
        
        await UpdateAsync(product, cancellationToken);
    }

    public async Task UpdatePriceAsync(Guid productId, decimal newPrice, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(productId, cancellationToken);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found");

        // Fiyat güncelleme mantığı
        // Örnek: product.Price = newPrice;
        
        await UpdateAsync(product, cancellationToken);
    }
}
