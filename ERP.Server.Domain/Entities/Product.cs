using ERP.Server.Domain.Entities.Common;
using ERP.Server.Domain.Entities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }
    
    [Required]
    public required ProductType Type { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; private set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; private set; }
    
    public int StockQuantity { get; private set; }
    
    [MaxLength(50)]
    public string? Sku { get; private set; }
    
    [MaxLength(100)]
    public string? Barcode { get; private set; }
    
    public bool IsActive { get; private set; } = true;
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? AvailableFrom { get; private set; }
    
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? AvailableTo { get; private set; }

/*
    * Product'ın adını ve tipini güncellemek için kullanıcı tarafından çağrılan metodumuz.
    * İçinde validasyonlar yapıyor ve alanları güncelliyor.
*/
    public void Update(string name, ProductType type, string? description = null, 
        decimal? price = null, int? stockQuantity = null, string? sku = null, 
        string? barcode = null, bool? isActive = null, 
        DateTime? availableFrom = null, DateTime? availableTo = null,
        string? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        Name = name.Trim();
        Type = type ?? throw new ArgumentNullException(nameof(type));
        
        if (description != null) Description = description;
        if (price.HasValue) Price = price.Value;
        if (stockQuantity.HasValue) StockQuantity = stockQuantity.Value;
        if (sku != null) Sku = sku;
        if (barcode != null) Barcode = barcode;
        if (isActive.HasValue) IsActive = isActive.Value;
        if (availableFrom.HasValue) AvailableFrom = availableFrom.Value;
        if (availableTo.HasValue) AvailableTo = availableTo.Value;
        
        SetUpdated(updatedBy);
    }
    
    public void UpdateStock(int quantity, string? updatedBy = null)
    {
        if (quantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(quantity));
            
        StockQuantity = quantity;
        SetUpdated(updatedBy);
    }
    
    public void UpdatePrice(decimal newPrice, string? updatedBy = null)
    {
        if (newPrice < 0)
            throw new ArgumentException("Price cannot be negative", nameof(newPrice));
            
        Price = newPrice;
        SetUpdated(updatedBy);
    }
    
    public void Activate(string? updatedBy = null)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }
    
    public void Deactivate(string? updatedBy = null)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }

    public new void Delete()
    {
        base.Delete();
    }
}
