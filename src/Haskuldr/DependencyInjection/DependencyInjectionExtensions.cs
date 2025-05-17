using System.Reflection;
using Haskuldr.Bus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haskuldr.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        services.AddScoped<IEventBus, EventBus>();
        
        var baseHandlerType = typeof(IEventHandler<>);
        
        foreach (var assembly in assemblies)
        {
            var serviceDescriptors = assembly.GetGenericServiceDescriptors(baseHandlerType);

            services.TryAddEnumerable(serviceDescriptors);
        }

        return services;
    }
}