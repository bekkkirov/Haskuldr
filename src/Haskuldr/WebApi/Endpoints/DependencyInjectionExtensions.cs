using System.Reflection;
using Haskuldr.DependencyInjection;
using Haskuldr.Shared;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haskuldr.WebApi.Endpoints;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddEndpoints(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        ThrowHelper.ThrowIfEmpty(assemblies);
        
        var baseEndpointType = typeof(IEndpoint);
        
        foreach (var assembly in assemblies)
        {
            var serviceDescriptors = assembly.GetServiceDescriptors(
                baseEndpointType,
                ServiceLifetime.Transient,
                true);

            services.TryAddEnumerable(serviceDescriptors);
        }
        
        return services;
    }

    public static IEndpointRouteBuilder MapEndpoints(
        this IEndpointRouteBuilder app,
        IServiceProvider serviceProvider)
    {
        var endpoints = serviceProvider.GetRequiredService<IEnumerable<IEndpoint>>();

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}