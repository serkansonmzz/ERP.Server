using ERP.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ERP.Server.Infrastructure.Data.Configurations; 

namespace ERP.Server.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    // Diğer DbSet'ler buraya eklenecek

   protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    base.OnModelCreating(modelBuilder);
    
    // Manuel olarak configuration'ları ekleyin
    modelBuilder.ApplyConfiguration(new ProductConfiguration());
    
    // Veya tüm configuration'ları otomatik olarak uygula
    // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
