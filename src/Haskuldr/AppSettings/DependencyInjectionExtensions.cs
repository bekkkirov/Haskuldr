using System.Reflection;
using Haskuldr.DependencyInjection;
using Haskuldr.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Haskuldr.AppSettings;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddAppSettings(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        ThrowHelper.ThrowIfEmpty(assemblies);

        var baseSettingType = typeof(IAppSetting);
        
        var propertyName = nameof(IAppSetting.Section);
        
        var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                     .GetMethods(BindingFlags.Public | BindingFlags.Static)
                     .First(m => 
                         m.Name == nameof(OptionsConfigurationServiceCollectionExtensions.Configure) &&
                         m.GetParameters().Length == 2);

        foreach (var assembly in assemblies)
        {
            var optionTypes = assembly
                              .GetConcreteTypes()
                              .Where(x => x.IsAssignableTo(baseSettingType));

            foreach (var optionType in optionTypes)
            {
                var sectionProperty = optionType.GetProperty(
                    propertyName,
                    BindingFlags.Public | BindingFlags.Static);

                if (sectionProperty is null)
                {
                    throw new InvalidOperationException($"'{propertyName}' property was not found on type '{optionType.FullName}'");
                }

                var sectionName = sectionProperty
                                  .GetValue(null)
                                  ?.ToString();

                if (string.IsNullOrWhiteSpace(sectionName))
                {
                    throw new InvalidOperationException($"'{propertyName}' property is null or empty on type '{optionType.FullName}'");
                }
                
                var configureMethod = method.MakeGenericMethod(optionType);

                var section = configuration.GetSection(sectionName);

                configureMethod.Invoke(null, [services, section]);
            }
        }

        return services;
    }
}