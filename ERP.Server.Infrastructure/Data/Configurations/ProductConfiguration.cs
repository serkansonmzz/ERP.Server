using ERP.Server.Domain.Entities;
using ERP.Server.Domain.Entities.Enums; 
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ERP.Server.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(p => p.Type)
            .IsRequired()
            .HasConversion(
                v => v.Value,  // Store the enum value
                v => ProductType.FromValue(v));  // Convert back to enum
            
        // Diğer property konfigürasyonları...
    }
}