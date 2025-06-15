using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums;

namespace ERP.Server.Domain.Interfaces.Repositories;

// Product Command Repository - Sadece yazma işlemleri için
public interface IProductCommandRepository : ICommandRepository<Product>
{
    // Product'a özel command'lar buraya eklenir
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken = default);
    Task UpdatePriceAsync(Guid productId, decimal newPrice, CancellationToken cancellationToken = default);
}

// Product Query Repository - Sadece okuma işlemleri için
public interface IProductQueryRepository : IQueryRepository<Product>
{
    // Product'a özel query'ler buraya eklenir
    Task<IReadOnlyList<Product>> GetProductsByTypeAsync(ProductType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
}

// Backward compatibility - Eski kodu kırmamak için bırakıyoruz, yeni kodlarda kullanmayın
[Obsolete("Use IProductCommandRepository and IProductQueryRepository instead for better CQRS separation")]
public interface IProductRepository : IRepository<Product>, IProductCommandRepository, IProductQueryRepository
{
}
