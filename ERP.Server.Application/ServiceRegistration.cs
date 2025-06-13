using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ERP.Server.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // MediatR pipeline'ına validasyon davranışını eklemek için daha sonra buraya döneceğiz.

        return services;
    }
}
