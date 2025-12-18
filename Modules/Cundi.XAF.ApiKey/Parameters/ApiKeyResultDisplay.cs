using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Cundi.XAF.ApiKey.Parameters;

/// <summary>
/// Non-persistent object for displaying generated API Key result.
/// Shown in a popup dialog that user must manually close.
/// </summary>
[DomainComponent]
public class ApiKeyResultDisplay
{
    /// <summary>
    /// The generated API Key value.
    /// </summary>
    [FieldSize(200)]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The user the key was generated for.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// When the key expires.
    /// </summary>
    public string ExpiresAt { get; set; } = string.Empty;

    /// <summary>
    /// Warning message for the user.
    /// </summary>
    [FieldSize(500)]
    public string Warning { get; set; } = "⚠️ IMPORTANT: Copy this key now! It will NOT be shown again.";
}
