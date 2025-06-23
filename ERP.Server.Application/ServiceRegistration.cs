// ERP.Server.Application/ServiceRegistration.cs
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ERP.Server.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddLogging(loggingBuilder => 
        {
            loggingBuilder.AddConfiguration(configuration.GetSection("Logging"));
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();
        });
        // AutoMapper'ı kaydeder
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // MediatR'ı kaydeder
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation'ı kaydeder
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Pipeline behavior'ları eklenebilir
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
       // Logging behavior'ını ekle
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }
}