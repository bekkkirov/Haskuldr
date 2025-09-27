using System.Reflection;
using Haskuldr.Abstractions.Validation;
using Haskuldr.DependencyInjection;
using Haskuldr.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haskuldr.Validation;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddValidators(
        this IServiceCollection services,
        ServiceLifetime lifeTime = ServiceLifetime.Transient,
        params Assembly[] assemblies)
    {
        ThrowHelper.ThrowIfEmpty(assemblies);
        
        var baseValidatorType = typeof(IValidator<>);
        
        foreach (var assembly in assemblies)
        {
            var serviceDescriptors = assembly.GetGenericServiceDescriptors(
                baseValidatorType,
                lifeTime);

            services.TryAddEnumerable(serviceDescriptors);
        }

        return services;
    }
}