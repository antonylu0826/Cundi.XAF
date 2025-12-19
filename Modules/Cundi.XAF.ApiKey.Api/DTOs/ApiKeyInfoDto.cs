namespace Cundi.XAF.ApiKey.Api.DTOs;

/// <summary>
/// DTO for API Key information query response.
/// </summary>
public class ApiKeyInfoDto
{
    /// <summary>
    /// The user's Oid associated with this API Key.
    /// </summary>
    public Guid UserOid { get; set; }

    /// <summary>
    /// Description or name of the API Key.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// When the API Key was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the API Key expires. Null means never expires.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// When the API Key was last used for authentication.
    /// </summary>
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    /// Whether the API Key is currently active.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Whether the API Key has expired.
    /// </summary>
    public bool IsExpired { get; set; }

    /// <summary>
    /// Whether the API Key is valid (active and not expired).
    /// </summary>
    public bool IsValid { get; set; }
}
