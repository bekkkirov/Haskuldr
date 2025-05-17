using System.Reflection;
using Haskuldr.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haskuldr.MinimalApi;

public static class DependencyInjection
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var baseEndpointType = typeof(IEndpoint);
        
        foreach (var assembly in assemblies)
        {
            var serviceDescriptors = assembly.GetServiceDescriptors(baseEndpointType, ServiceLifetime.Transient);

            services.TryAddEnumerable(serviceDescriptors);
        }
        
        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}