using Haskuldr.EventSystem;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haskuldr.Mediator;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        Action<MediatorConfiguration> configure)
    {
        var configuration = new MediatorConfiguration();
        configure.Invoke(configuration);
        
        services.AddMediator(configuration);

        return services;
    }

    private static void AddMediator(
        this IServiceCollection services,
        MediatorConfiguration configuration)
    {
        services.Add(new ServiceDescriptor(typeof(IMediator), typeof(Mediator), configuration.Lifetime));
        
        services.TryAddEnumerable(configuration.Handlers);

        foreach (var decorator in configuration.Decorators)
        {
            services.DecorateRequestHandler(decorator);
        }
    }
    
    private static IServiceCollection DecorateRequestHandler(
        this IServiceCollection services,
        Type decoratorType)
    {
        if (!decoratorType.IsGenericTypeDefinition)
        {
            throw new InvalidOperationException($"Decorator {decoratorType.FullName} is not an open generic type");
        }

        var requests = new List<Type> {
            typeof(IRequestHandler<>),
            typeof(IRequestHandler<,>)
        };

        var serviceType = decoratorType
                          .GetInterfaces()
                          .SingleOrDefault(x => 
                              x.IsGenericType && 
                              requests.Contains(x.GetGenericTypeDefinition()));

        if (serviceType is null)
        {
            throw new InvalidOperationException($"Decorator {decoratorType.FullName} does not implement IRequestHandler interface");
        }
        
        var descriptors = services
                          .Where(s =>
                              s.ServiceType.IsGenericType &&
                              s.ServiceType.GetGenericTypeDefinition() == serviceType.GetGenericTypeDefinition())
                          .ToList();

        if (descriptors.Count == 0)
        {
            throw new InvalidOperationException(
                $"No service types matching generic pattern {serviceType.FullName} are registered");
        }

        foreach (var descriptor in descriptors)
        {
            var newDecoratorType = decoratorType.MakeGenericType(descriptor.ServiceType.GenericTypeArguments);

            services.DecorateInternal(serviceType, newDecoratorType);
        }

        return services;
    }
    
    private static IServiceCollection DecorateInternal(
        this IServiceCollection services,
        Type serviceType,
        Type decoratorType)
    {
        var descriptor = services.SingleOrDefault(s => s.ServiceType == serviceType);

        if (descriptor is null)
        {
            throw new InvalidOperationException($"Service type {serviceType.FullName} is not registered");
        }

        services.Remove(descriptor);

        services.Add(new ServiceDescriptor(
            serviceType,
            serviceProvider =>
            {
                var original = GetInstanceFromDescriptor(serviceProvider, descriptor);

                return ActivatorUtilities.CreateInstance(serviceProvider, decoratorType, original);
            },
            descriptor.Lifetime));

        return services;
    }
    
    private static object GetInstanceFromDescriptor(
        IServiceProvider serviceProvider,
        ServiceDescriptor descriptor)
    {
        object instance;

        if (descriptor.ImplementationType is not null)
        {
            instance = ActivatorUtilities.CreateInstance(serviceProvider, descriptor.ImplementationType);
        }
        else if (descriptor.ImplementationInstance is not null)
        {
            instance = descriptor.ImplementationInstance;
        }
        else if (descriptor.ImplementationFactory is not null)
        {
            instance = descriptor.ImplementationFactory(serviceProvider);
        }
        else
        {
            throw new InvalidOperationException($"Service type {descriptor.ImplementationType} is not registered");
        }

        return instance;
    }
}