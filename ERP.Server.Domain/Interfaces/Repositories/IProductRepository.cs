using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums;

namespace ERP.Server.Domain.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product>
{
    // Product'a özel sorgular buraya eklenecek
    Task<IReadOnlyList<Product>> GetProductsByTypeAsync(ProductType type, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
}
