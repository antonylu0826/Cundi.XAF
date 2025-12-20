using Cundi.XAF.Core.Api;
using Cundi.XAF.Core.Api.Extensions;
using Cundi.XAF.Triggers.Api;
using Microsoft.Extensions.DependencyInjection;

namespace Cundi.XAF.Triggers.Extensions;

/// <summary>
/// Extension methods for adding Triggers services to ASP.NET Core WebApi applications.
/// </summary>
public static class TriggersExtensions
{
    /// <summary>
    /// Adds Triggers services to the service collection for WebApi.
    /// This registers the TriggerDataServicePlugin to enable trigger processing on object changes.
    /// Note: You must also call AddCompositeDataService() after registering all plugins.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddTriggers(this IServiceCollection services)
    {
        // Register the Trigger plugin
        // This intercepts ObjectSpace commit events to detect changes and invoke webhooks
        services.AddDataServicePlugin<TriggerDataServicePlugin>();

        return services;
    }
}
