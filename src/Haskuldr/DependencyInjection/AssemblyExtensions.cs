using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Haskuldr.DependencyInjection;

public static class AssemblyExtensions
{
    public static IEnumerable<ServiceDescriptor> GetGenericServiceDescriptors(
        this Assembly assembly,
        Type baseType,
        ServiceLifetime lifeTime = ServiceLifetime.Scoped)
    {
        return assembly
               .ExportedTypes
               .Where(type => type is { IsAbstract: false, IsInterface: false })
               .Select(type => new
               {
                   ServiceType = type
                                 .GetInterfaces()
                                 .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseType),
                   ImplementationType = type,
               })
               .Where(x => x.ServiceType is not null)
               .Select(x => ServiceDescriptor.Describe(x.ServiceType!, x.ImplementationType, lifeTime));
    }

    public static IEnumerable<ServiceDescriptor> GetServiceDescriptors(
        this Assembly assembly,
        Type baseType,
        ServiceLifetime lifeTime = ServiceLifetime.Scoped)
    {
        return assembly
               .ExportedTypes
               .Where(type => type is { IsAbstract: false, IsInterface: false } && type.IsAssignableTo(baseType))
               .Select(type => ServiceDescriptor.Describe(baseType, type, lifeTime));
    }
}