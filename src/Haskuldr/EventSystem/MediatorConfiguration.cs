using System.Reflection;
using Haskuldr.DependencyInjection;
using Haskuldr.Mediator;
using Haskuldr.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Haskuldr.EventSystem;

public sealed class MediatorConfiguration
{
    private static readonly Type[] _baseHandlerTypes =
    [
        typeof(IRequestHandler<>),
        typeof(IRequestHandler<,>)
    ];

    private readonly List<ServiceDescriptor> _handlers = [];
    private readonly List<Type> _decorators = [];

    public static IReadOnlyCollection<Type> BaseHandlerTypes => Array.AsReadOnly(_baseHandlerTypes);

    public ServiceLifetime Lifetime { get; private set; } = ServiceLifetime.Transient;

    public IReadOnlyCollection<ServiceDescriptor> Handlers => _handlers.AsReadOnly();

    public IReadOnlyCollection<Type> Decorators => _decorators.AsReadOnly();

    public MediatorConfiguration WithLifetime(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
        
        return this;
    }
    
    public MediatorConfiguration WithHandlersFromAssembly(
        Assembly assembly)
    {
        var serviceDescriptors = assembly
                                 .GetGenericServiceDescriptors(_baseHandlerTypes, Lifetime)
                                 .Where(x => x.ServiceType.GenericTypeArguments.All(a => !a.IsGenericParameter))
                                 .ToList();

        _handlers.AddRange(serviceDescriptors);

        return this;
    }

    public MediatorConfiguration WithHandlersFromAssemblies(params Assembly[] assemblies)
    {
        ThrowHelper.ThrowIfEmpty(assemblies);
        
        foreach (var assembly in assemblies)
        {
            WithHandlersFromAssembly(assembly);
        }

        return this;
    }

    public MediatorConfiguration WithDecorator(Type decorator)
    {
        _decorators.Add(decorator);
        
        return this;
    }
}
