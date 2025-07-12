using System.Reflection;
using Haskuldr.DependencyInjection;
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
        if (assemblies.Length == 0)
        {
            throw new ArgumentException("At least one assembly must be provided", nameof(assemblies));
        }
        
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