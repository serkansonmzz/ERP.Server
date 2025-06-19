// ERP.Server.Application/ServiceRegistration.cs
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using ERP.Server.Application.Behaviors;

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

        // Add pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        
        // Uncomment and implement validation behavior when ready
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        return services;
    }
}