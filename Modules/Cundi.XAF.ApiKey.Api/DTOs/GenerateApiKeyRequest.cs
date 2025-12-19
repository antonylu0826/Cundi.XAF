using Cundi.XAF.ApiKey.Parameters;

namespace Cundi.XAF.ApiKey.Api.DTOs;

/// <summary>
/// Request DTO for generating a new API Key.
/// </summary>
public class GenerateApiKeyRequest
{
    /// <summary>
    /// The target user's Oid (Guid).
    /// </summary>
    public Guid UserOid { get; set; }

    /// <summary>
    /// Expiration option using the existing ApiKeyExpiration enum.
    /// Defaults to 30 days.
    /// </summary>
    public ApiKeyExpiration Expiration { get; set; } = ApiKeyExpiration.Days30;

    /// <summary>
    /// Optional description for the API Key.
    /// </summary>
    public string? Description { get; set; }
}
