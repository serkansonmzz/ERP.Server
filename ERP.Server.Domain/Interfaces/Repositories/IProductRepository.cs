using ERP.Server.Domain.Entities;

namespace ERP.Server.Domain.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    // Product'a özel sorgular buraya eklenecek
    Task<IReadOnlyList<Product>> GetProductsByTypeAsync(Enums.ProductType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
}
