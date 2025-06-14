using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Tüm Infrastructure servisleri burada kaydedilecek
        // Örnek: var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        return services;
    }
}
