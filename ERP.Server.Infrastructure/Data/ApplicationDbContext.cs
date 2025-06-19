using ERP.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ERP.Server.Infrastructure.Data.Configurations;  
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using ERP.Server.Application.Interfaces;
using ERP.Server.Domain.Entities.Common;

namespace ERP.Server.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{

    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService? currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

   

    public DbSet<Product> Products { get; set; }
    public DbSet<OutBox> OutBoxes { get; set; }
    // Diğer DbSet'ler buraya eklenecek

   protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
   
      base.OnModelCreating(modelBuilder);

       foreach (var entityType in modelBuilder.Model.GetEntityTypes()) 
       {
           if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
           { 
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
                var condition = Expression.MakeBinary(ExpressionType.Equal, property, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);
                    
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
           
           }
               
       } 
    
    // Manuel olarak configuration'ları ekleyin
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    
    // Veya tüm configuration'ları otomatik olarak uygula
    // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }


     public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Entity && 
                      (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

            var currentTime = DateTime.UtcNow;
            var currentUser = _currentUserService?.UserId;

            foreach (var entry in entries)
            {
                var entity = (Entity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = currentTime;
                    if (currentUser != null)
                        entity.CreatedBy = currentUser;
                }

                if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                {
                    entity.SetUpdated(currentUser);

                    if (entry.State == EntityState.Deleted && entity is ISoftDelete)
                    {
                        entry.State = EntityState.Modified;
                        entity.Delete(currentUser);
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
    }

}
