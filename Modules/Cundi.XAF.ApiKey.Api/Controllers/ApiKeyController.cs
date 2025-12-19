using Cundi.XAF.ApiKey.Api.DTOs;
using Cundi.XAF.ApiKey.BusinessObjects;
using Cundi.XAF.ApiKey.Parameters;
using Cundi.XAF.ApiKey.Services;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cundi.XAF.ApiKey.Api.Controllers;

/// <summary>
/// API controller for managing API Keys.
/// Provides endpoints to generate, query, and revoke API keys.
/// Requires administrator role for all operations.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ApiKeyController : ControllerBase
{
    private readonly INonSecuredObjectSpaceFactory _objectSpaceFactory;
    private readonly ISecurityStrategyBase _securitySystem;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyController"/> class.
    /// </summary>
    /// <param name="objectSpaceFactory">Factory for creating non-secured object spaces.</param>
    /// <param name="securitySystem">The XAF security system.</param>
    public ApiKeyController(
        INonSecuredObjectSpaceFactory objectSpaceFactory,
        ISecurityStrategyBase securitySystem)
    {
        _objectSpaceFactory = objectSpaceFactory;
        _securitySystem = securitySystem;
    }

    /// <summary>
    /// Generates a new API Key for the specified user.
    /// Any existing API Key for the user will be replaced.
    /// </summary>
    /// <param name="request">The API Key generation request containing user OID and expiration settings.</param>
    /// <returns>
    /// Returns the generated API Key on success. The API Key is only shown once and cannot be retrieved later.
    /// </returns>
    /// <response code="200">API Key generated successfully.</response>
    /// <response code="400">User not found.</response>
    /// <response code="403">Caller is not an administrator.</response>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(GenerateApiKeyResponse), 200)]
    [ProducesResponseType(typeof(GenerateApiKeyResponse), 400)]
    [ProducesResponseType(typeof(GenerateApiKeyResponse), 403)]
    public IActionResult Generate([FromBody] GenerateApiKeyRequest request)
    {
        if (!IsCurrentUserAdministrator())
        {
            return Forbidden("Only administrators can manage API Keys.");
        }

        using var objectSpace = _objectSpaceFactory.CreateNonSecuredObjectSpace(typeof(ApiKeyInfo));

        var targetUser = objectSpace.GetObjectByKey(_securitySystem.UserType, request.UserOid);
        if (targetUser == null)
        {
            return BadRequest(new GenerateApiKeyResponse
            {
                Success = false,
                Message = $"User with Oid '{request.UserOid}' not found."
            });
        }

        var userName = (targetUser as ISecurityUser)?.UserName ?? request.UserOid.ToString();

        // Remove existing API key if present
        var existingKey = FindApiKeyByUserOid(objectSpace, request.UserOid);
        if (existingKey != null)
        {
            objectSpace.Delete(existingKey);
        }

        // Generate new API key
        var (plaintextKey, hashValue) = ApiKeyGenerator.Generate();

        var apiKeyInfo = objectSpace.CreateObject<ApiKeyInfo>();
        apiKeyInfo.UserOid = request.UserOid;
        apiKeyInfo.ApiKeyHash = hashValue;
        apiKeyInfo.Description = request.Description ?? $"API Key for {userName}";
        apiKeyInfo.ExpiresAt = CalculateExpirationDate(request.Expiration);

        objectSpace.CommitChanges();

        return Ok(new GenerateApiKeyResponse
        {
            Success = true,
            ApiKey = plaintextKey,
            ExpiresAt = apiKeyInfo.ExpiresAt,
            Message = "API Key generated successfully. Save this key now - it will not be shown again."
        });
    }

    /// <summary>
    /// Gets API Key information for the specified user.
    /// </summary>
    /// <param name="userOid">The user's Oid (GUID).</param>
    /// <returns>API Key information including creation date, expiration, and status.</returns>
    /// <response code="200">API Key information retrieved successfully.</response>
    /// <response code="403">Caller is not an administrator.</response>
    /// <response code="404">No API Key found for the specified user.</response>
    [HttpGet("{userOid:guid}")]
    [ProducesResponseType(typeof(ApiKeyInfoDto), 200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public IActionResult Get(Guid userOid)
    {
        if (!IsCurrentUserAdministrator())
        {
            return Forbidden("Only administrators can view API Key information.");
        }

        using var objectSpace = _objectSpaceFactory.CreateNonSecuredObjectSpace(typeof(ApiKeyInfo));

        var apiKeyInfo = FindApiKeyByUserOid(objectSpace, userOid);
        if (apiKeyInfo == null)
        {
            return NotFound(new { message = $"No API Key found for user with Oid '{userOid}'." });
        }

        return Ok(MapToDto(apiKeyInfo));
    }

    /// <summary>
    /// Revokes the API Key for the specified user.
    /// </summary>
    /// <param name="userOid">The user's Oid (GUID).</param>
    /// <returns>Success message if the API Key was revoked.</returns>
    /// <response code="200">API Key revoked successfully.</response>
    /// <response code="403">Caller is not an administrator.</response>
    /// <response code="404">No API Key found for the specified user.</response>
    [HttpDelete("{userOid:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public IActionResult Revoke(Guid userOid)
    {
        if (!IsCurrentUserAdministrator())
        {
            return Forbidden("Only administrators can revoke API Keys.");
        }

        using var objectSpace = _objectSpaceFactory.CreateNonSecuredObjectSpace(typeof(ApiKeyInfo));

        var apiKeyInfo = FindApiKeyByUserOid(objectSpace, userOid);
        if (apiKeyInfo == null)
        {
            return NotFound(new { message = $"No API Key found for user with Oid '{userOid}'." });
        }

        objectSpace.Delete(apiKeyInfo);
        objectSpace.CommitChanges();

        return Ok(new { success = true, message = $"API Key for user with Oid '{userOid}' has been revoked." });
    }

    #region Private Helper Methods

    /// <summary>
    /// Finds an API Key by user OID.
    /// </summary>
    private static ApiKeyInfo? FindApiKeyByUserOid(IObjectSpace objectSpace, Guid userOid)
    {
        return objectSpace.FindObject<ApiKeyInfo>(CriteriaOperator.Parse("UserOid = ?", userOid));
    }

    /// <summary>
    /// Maps an ApiKeyInfo entity to its DTO.
    /// </summary>
    private static ApiKeyInfoDto MapToDto(ApiKeyInfo apiKeyInfo)
    {
        return new ApiKeyInfoDto
        {
            UserOid = apiKeyInfo.UserOid,
            Description = apiKeyInfo.Description,
            CreatedAt = apiKeyInfo.CreatedAt,
            ExpiresAt = apiKeyInfo.ExpiresAt,
            LastUsedAt = apiKeyInfo.LastUsedAt,
            IsActive = apiKeyInfo.IsActive,
            IsExpired = apiKeyInfo.IsExpired,
            IsValid = apiKeyInfo.IsValid
        };
    }

    /// <summary>
    /// Returns a 403 Forbidden response with a consistent format.
    /// </summary>
    private ObjectResult Forbidden(string message)
    {
        return StatusCode(403, new GenerateApiKeyResponse
        {
            Success = false,
            Message = message
        });
    }

    /// <summary>
    /// Checks if the current user has an administrative role.
    /// </summary>
    private bool IsCurrentUserAdministrator()
    {
        try
        {
            if (_securitySystem?.User == null)
                return false;

            var rolesProperty = _securitySystem.User.GetType().GetProperty("Roles");
            if (rolesProperty == null)
                return false;

            if (rolesProperty.GetValue(_securitySystem.User) is not System.Collections.IEnumerable roles)
                return false;

            foreach (var role in roles)
            {
                if (role is PermissionPolicyRole { IsAdministrative: true })
                    return true;

                var isAdminProperty = role.GetType().GetProperty("IsAdministrative");
                if (isAdminProperty?.GetValue(role) is true)
                    return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Calculates the expiration date based on the ApiKeyExpiration enum value.
    /// </summary>
    private static DateTime CalculateExpirationDate(ApiKeyExpiration expiration)
    {
        return DateTime.UtcNow.AddMinutes((int)expiration);
    }

    #endregion
}
