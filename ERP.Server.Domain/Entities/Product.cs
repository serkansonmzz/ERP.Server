using ERP.Server.Domain.Entities.Common;
using ERP.Server.Domain.Entities.Enums;

namespace ERP.Server.Domain.Entities;

public sealed class Product : Entity
{
    private Product() { } // For EF Core

    public Product(string name, ProductType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        Name = name.Trim();
        Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public string Name { get; private set; }
    public ProductType Type { get; private set; }

/*
    * Product'ın adını ve tipini güncellemek için kullanıcı tarafından çağrılan metodumuz.
    * İçinde validasyonlar yapıyor ve alanları güncelliyor.
*/
    public void Update(string name, ProductType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        Name = name.Trim();
        Type = type ?? throw new ArgumentNullException(nameof(type));
        SetUpdated();//Teme Entity classındaki method.
    }

    public new void Delete()
    {
        base.Delete();
    }
}
