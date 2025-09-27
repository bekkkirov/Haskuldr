using System.Reflection;
using Haskuldr.DependencyInjection;
using Haskuldr.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haskuldr.EventSystem;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        ServiceLifetime lifeTime = ServiceLifetime.Scoped,
        params Assembly[] assemblies)
    {
        ThrowHelper.ThrowIfEmpty(assemblies);
        
        services.AddScoped<IEventBus, EventBus>();
        
        var baseHandlerType = typeof(IEventHandler<>);
        
        foreach (var assembly in assemblies)
        {
            var serviceDescriptors = assembly.GetGenericServiceDescriptors(
                baseHandlerType,
                lifeTime);

            services.TryAddEnumerable(serviceDescriptors);
        }

        return services;
    }
}