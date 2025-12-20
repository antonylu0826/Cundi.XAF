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
    /// This registers MirrorTypeMappings and MirrorService for dependency injection.
    /// Type mappings are configured via the XAF UI (MirrorTypeMappingConfig entity).
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDataMirror(this IServiceCollection services)
    {
        // Register MirrorTypeMappings as scoped (reads from database dynamically)
        services.AddScoped<MirrorTypeMappings>();

        // Register MirrorService as scoped (needs INonSecuredObjectSpaceFactory which is scoped)
        services.AddScoped<MirrorService>();

        return services;
    }
}
