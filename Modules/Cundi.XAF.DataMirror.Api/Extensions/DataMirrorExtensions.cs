#nullable enable
using Cundi.XAF.Core.Api.Extensions;
using Cundi.XAF.DataMirror.Api.Services;
using Cundi.XAF.DataMirror.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cundi.XAF.DataMirror.Extensions;

/// <summary>
/// Extension methods for adding DataMirror services to ASP.NET Core applications.
/// </summary>
public static class DataMirrorExtensions
{
    /// <summary>
    /// Adds DataMirror services to the service collection.
    /// This registers:
    /// - MirrorTypeMappings: Dynamic type mapping service
    /// - MirrorService: Mirror payload processing service
    /// - MirroredObjectDataServicePlugin: Protects MirroredObject-derived types from API modifications
    /// Note: You must also call AddCompositeDataService() after registering all plugins.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDataMirror(this IServiceCollection services)
    {
        // Register MirrorTypeMappings as scoped (reads from database dynamically)
        services.AddScoped<MirrorTypeMappings>();

        // Register MirrorService as scoped (needs INonSecuredObjectSpaceFactory which is scoped)
        services.AddScoped<MirrorService>();

        // Register MirroredObjectDataServicePlugin to protect MirroredObject-derived types
        // This blocks Create/Update/Delete operations on any type that inherits from MirroredObject
        services.AddDataServicePlugin<MirroredObjectDataServicePlugin>();

        return services;
    }
}
