using Microsoft.Extensions.DependencyInjection;

namespace ERP.Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Tüm Infrastructure servisleri burada kaydedilecek
        return services;
    }
}
