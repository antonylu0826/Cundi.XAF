namespace Cundi.XAF.ApiKey.Api.DTOs;

/// <summary>
/// Response DTO for API Key generation.
/// </summary>
public class GenerateApiKeyResponse
{
    /// <summary>
    /// Whether the API Key was successfully generated.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The plaintext API Key. Only shown once - not stored in database.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// When the API Key expires. Null means never expires.
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Additional message or error description.
    /// </summary>
    public string? Message { get; set; }
}
