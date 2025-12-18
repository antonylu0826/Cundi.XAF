using Cundi.XAF.ApiKey.Api.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace Cundi.XAF.ApiKey.Api.Extensions;

/// <summary>
/// Extension methods for adding API Key authentication to ASP.NET Core applications.
/// </summary>
public static class ApiKeyExtensions
{
    /// <summary>
    /// Adds API Key authentication to the authentication builder.
    /// </summary>
    /// <param name="builder">The authentication builder</param>
    /// <returns>The authentication builder for chaining</returns>
    public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder)
    {
        return builder.AddApiKey(ApiKeyAuthenticationHandler.SchemeName, _ => { });
    }

    /// <summary>
    /// Adds API Key authentication to the authentication builder with a custom scheme name.
    /// </summary>
    /// <param name="builder">The authentication builder</param>
    /// <param name="authenticationScheme">The authentication scheme name</param>
    /// <param name="configureOptions">Configuration options</param>
    /// <returns>The authentication builder for chaining</returns>
    public static AuthenticationBuilder AddApiKey(
        this AuthenticationBuilder builder,
        string authenticationScheme,
        Action<ApiKeyAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
            authenticationScheme,
            configureOptions);
    }
}
