using System.Reflection;
using Haskuldr.DependencyInjection;
using Haskuldr.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haskuldr.WebApi.Endpoints;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        ServiceLifetime lifeTime = ServiceLifetime.Transient,
        params Assembly[] assemblies)
    {
        ThrowHelper.ThrowIfEmpty(assemblies);
        
        var baseEndpointType = typeof(IEndpoint);
        
        foreach (var assembly in assemblies)
        {
            var serviceDescriptors = assembly.GetServiceDescriptors(
                baseEndpointType,
                lifeTime);

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