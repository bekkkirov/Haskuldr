using System.Reflection;
using Haskuldr.DependencyInjection;
using Haskuldr.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haskuldr.Mediator;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        ServiceLifetime lifeTime = ServiceLifetime.Transient,
        params Assembly[] assemblies)
    {
        ThrowHelper.ThrowIfEmpty(assemblies);
        
        services.AddScoped<IMediator, Mediator>();
        
        var baseHandlerTypes = new [] {typeof(IRequestHandler<>), typeof(IRequestHandler<,>)};
        
        foreach (var assembly in assemblies)
        {
            var serviceDescriptors = assembly.GetGenericServiceDescriptors(
                baseHandlerTypes,
                lifeTime);

            services.TryAddEnumerable(serviceDescriptors);
        }

        return services;
    }
}