#nullable enable
using DevExpress.ExpressApp.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cundi.XAF.Core.Api.Extensions;

/// <summary>
/// Extension methods for adding Core API services to ASP.NET Core applications.
/// </summary>
public static class CoreApiExtensions
{
    /// <summary>
    /// Adds the CompositeDataService to the service collection.
    /// This replaces the default IDataService with CompositeDataService
    /// which delegates to all registered IDataServicePlugin implementations.
    /// Call this AFTER registering all module-specific plugins.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddCompositeDataService(this IServiceCollection services)
    {
        services.AddScoped<IDataService, CompositeDataService>();
        return services;
    }

    /// <summary>
    /// Registers a DataService plugin.
    /// Multiple plugins can be registered and they will all be invoked
    /// when ObjectSpace is created.
    /// </summary>
    /// <typeparam name="T">The plugin type implementing IDataServicePlugin</typeparam>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDataServicePlugin<T>(this IServiceCollection services)
        where T : class, IDataServicePlugin
    {
        services.AddScoped<IDataServicePlugin, T>();
        return services;
    }
}
