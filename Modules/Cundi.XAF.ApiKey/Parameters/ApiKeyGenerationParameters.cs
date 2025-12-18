using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace Cundi.XAF.ApiKey.Parameters;

/// <summary>
/// Enumeration for API Key expiration options.
/// </summary>
public enum ApiKeyExpiration
{
    [XafDisplayName("5 Minutes")]
    Minutes5 = 5,

    [XafDisplayName("30 Minutes")]
    Minutes30 = 30,

    [XafDisplayName("1 Day")]
    Day1 = 1440,  // 24 * 60 minutes

    [XafDisplayName("30 Days")]
    Days30 = 43200,  // 30 * 24 * 60 minutes

    [XafDisplayName("60 Days")]
    Days60 = 86400,  // 60 * 24 * 60 minutes

    [XafDisplayName("90 Days")]
    Days90 = 129600  // 90 * 24 * 60 minutes
}

/// <summary>
/// Non-persistent object for API Key generation parameters.
/// Used as a popup dialog to collect expiration settings.
/// </summary>
[DomainComponent]
public class ApiKeyGenerationParameters
{
    /// <summary>
    /// The selected expiration option for the new API Key.
    /// </summary>
    [ImmediatePostData]
    public ApiKeyExpiration Expiration { get; set; } = ApiKeyExpiration.Days30;

    /// <summary>
    /// Calculates the expiration date based on the selected option.
    /// </summary>
    public DateTime GetExpirationDate()
    {
        return DateTime.UtcNow.AddMinutes((int)Expiration);
    }
}
