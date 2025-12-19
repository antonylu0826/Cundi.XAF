using Cundi.XAF.SyncReceiver.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cundi.XAF.SyncReceiver.Extensions;

/// <summary>
/// Extension methods for adding SyncReceiver services to ASP.NET Core applications.
/// </summary>
public static class SyncReceiverExtensions
{
    /// <summary>
    /// Adds SyncReceiver services to the service collection.
    /// This registers SyncTypeMappings and SyncService for dependency injection.
    /// Type mappings are configured via the XAF UI (SyncTypeMappingConfig entity).
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSyncReceiver(this IServiceCollection services)
    {
        // Register SyncTypeMappings as scoped (reads from database dynamically)
        services.AddScoped<SyncTypeMappings>();

        // Register SyncService as scoped (needs INonSecuredObjectSpaceFactory which is scoped)
        services.AddScoped<SyncService>();

        return services;
    }
}
