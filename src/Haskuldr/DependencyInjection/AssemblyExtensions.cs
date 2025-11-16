using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Haskuldr.DependencyInjection;

public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetConcreteTypes(
        this Assembly assembly,
        bool includeNonPublic = false)
    {
        var types = includeNonPublic 
            ? assembly.DefinedTypes
            : assembly.ExportedTypes;
        
        return types.Where(type => type is { IsClass: true, IsAbstract: false });
    }
    
    public static IEnumerable<ServiceDescriptor> GetGenericServiceDescriptors(
        this Assembly assembly,
        Type baseType,
        ServiceLifetime lifeTime = ServiceLifetime.Scoped)
    {
        return assembly
               .GetConcreteTypes()
               .Select(type => new ServiceMapping(
                   type
                       .GetInterfaces()
                       .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType),
                   type))
               .ToServiceDescriptors(lifeTime);
    }

    public static IEnumerable<ServiceDescriptor> GetGenericServiceDescriptors(
        this Assembly assembly,
        Type[] baseTypes,
        ServiceLifetime lifeTime = ServiceLifetime.Scoped)
    {
        return assembly
               .GetConcreteTypes()
               .Select(type => new ServiceMapping(
                   type
                       .GetInterfaces()
                       .FirstOrDefault(i => i.IsGenericType && baseTypes.Contains(i.GetGenericTypeDefinition())),
                   type))
               .ToServiceDescriptors(lifeTime);
    }

    public static IEnumerable<ServiceDescriptor> GetServiceDescriptors(
        this Assembly assembly,
        Type baseType,
        ServiceLifetime lifeTime = ServiceLifetime.Scoped,
        bool includeNonPublic = false)
    {
        return assembly
               .GetConcreteTypes(includeNonPublic)
               .Where(type => type.IsAssignableTo(baseType))
               .Select(type => ServiceDescriptor.Describe(baseType, type, lifeTime));
    }

    private static IEnumerable<ServiceDescriptor> ToServiceDescriptors(
        this IEnumerable<ServiceMapping> types,
        ServiceLifetime lifeTime)
    {
        return types
               .Where(x => x.ServiceType is not null)
               .Select(type => ServiceDescriptor.Describe(type.ServiceType!, type.ImplementationType, lifeTime));
    }
}