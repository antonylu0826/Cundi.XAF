using Cundi.XAF.Triggers.Api;
using DevExpress.ExpressApp.WebApi.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cundi.XAF.Triggers.Extensions;

/// <summary>
/// Extension methods for adding Triggers services to ASP.NET Core WebApi applications.
/// </summary>
public static class TriggersExtensions
{
    /// <summary>
    /// Adds Triggers services to the service collection for WebApi.
    /// This replaces the default IDataService with WebApiMiddleDataService
    /// to enable trigger processing on object changes.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddTriggers(this IServiceCollection services)
    {
        // Replace the default DataService with WebApiMiddleDataService
        // This intercepts ObjectSpace commit events to detect changes and invoke webhooks
        services.AddScoped<IDataService, WebApiMiddleDataService>();

        return services;
    }
}
