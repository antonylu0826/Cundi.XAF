using Cundi.XAF.DataMirror.Api.Services;
using Cundi.XAF.DataMirror.Services;
using DevExpress.ExpressApp.WebApi.Services;
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
    /// - MirroredObjectDataService: Protects MirroredObject-derived types from API modifications
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDataMirror(this IServiceCollection services)
    {
        // Register MirrorTypeMappings as scoped (reads from database dynamically)
        services.AddScoped<MirrorTypeMappings>();

        // Register MirrorService as scoped (needs INonSecuredObjectSpaceFactory which is scoped)
        services.AddScoped<MirrorService>();

        // Register MirroredObjectDataService to protect MirroredObject-derived types
        // This replaces the default IDataService and blocks Create/Update/Delete operations
        // on any type that inherits from MirroredObject
        services.AddScoped<IDataService, MirroredObjectDataService>();

        return services;
    }
}
