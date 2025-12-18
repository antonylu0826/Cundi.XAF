using Cundi.XAF.ApiKey.Services;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Cundi.XAF.ApiKey.Api.Authentication;

/// <summary>
/// Authentication handler for API Key authentication.
/// Integrates with XAF Security System using SignInManager.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public const string SchemeName = "ApiKey";
    public const string HeaderName = "X-API-Key";

    private readonly IServiceProvider _serviceProvider;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IServiceProvider serviceProvider)
        : base(options, logger, encoder)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Check for API key header
        if (!Request.Headers.TryGetValue(HeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(providedApiKey))
        {
            return AuthenticateResult.NoResult();
        }

        try
        {
            // Get ObjectSpaceFactory from services
            var objectSpaceFactory = _serviceProvider.GetService(typeof(INonSecuredObjectSpaceFactory)) as INonSecuredObjectSpaceFactory;
            if (objectSpaceFactory == null)
            {
                Logger.LogError("INonSecuredObjectSpaceFactory not available for API Key authentication.");
                return AuthenticateResult.Fail("Authentication service unavailable.");
            }

            // Create non-secured object space for validation
            using var objectSpace = objectSpaceFactory.CreateNonSecuredObjectSpace(typeof(BusinessObjects.ApiKeyInfo));

            // Validate the API key
            var validator = new ApiKeyValidator();
            var result = validator.Validate(objectSpace, providedApiKey);

            if (!result.IsValid)
            {
                Logger.LogWarning("API Key authentication failed: {Error}", result.ErrorMessage);
                return AuthenticateResult.Fail(result.ErrorMessage ?? "Invalid API key.");
            }

            // Get SignInManager to authenticate with XAF Security System
            var signInManager = _serviceProvider.GetService(typeof(SignInManager)) as SignInManager;
            if (signInManager == null)
            {
                Logger.LogError("SignInManager not available for API Key authentication.");
                return AuthenticateResult.Fail("SignInManager unavailable.");
            }

            // Get the security system
            var securitySystem = _serviceProvider.GetService(typeof(ISecurityStrategyBase)) as ISecurityStrategyBase;
            if (securitySystem == null)
            {
                Logger.LogError("ISecurityStrategyBase not available for API Key authentication.");
                return AuthenticateResult.Fail("Security system unavailable.");
            }

            // Find the user by Oid
            var userOid = result.UserOid!.Value;
            var user = objectSpace.GetObjectByKey(securitySystem.UserType, userOid) as ISecurityUser;
            if (user == null)
            {
                Logger.LogWarning("User with Oid {UserOid} not found for API Key.", userOid);
                return AuthenticateResult.Fail("User not found.");
            }

            // Get username from user
            var userName = user.UserName;

            // Create ClaimsPrincipal for the user using SignInManager
            var userPrincipal = signInManager.CreateUserPrincipal(user);

            // Sign in using the principal
            var authResult = signInManager.SignInByPrincipal(userPrincipal);
            if (!authResult.Succeeded)
            {
                var errorMessage = authResult.Error?.Message ?? "Authentication failed.";
                Logger.LogWarning("XAF authentication failed for user {UserName}: {Error}", userName, errorMessage);
                return AuthenticateResult.Fail($"Login failed for '{userName}'.");
            }

            // Use the claims from XAF SignInManager result
            var principal = authResult.Principal;
            var ticket = new AuthenticationTicket(principal, SchemeName);

            Logger.LogInformation("API Key authentication successful for user {UserName}.", userName);
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during API Key authentication.");
            return AuthenticateResult.Fail("Authentication error.");
        }
    }
}

/// <summary>
/// Options for API Key authentication.
/// </summary>
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
}
