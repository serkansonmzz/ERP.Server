// ERP.Server.Application/ServiceRegistration.cs
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ERP.Server.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper'ı kaydeder
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // MediatR'ı kaydeder
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation'ı kaydeder
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Pipeline behavior'ları eklenebilir
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}