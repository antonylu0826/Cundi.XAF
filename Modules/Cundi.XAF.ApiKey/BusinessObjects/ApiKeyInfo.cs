using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Cundi.XAF.ApiKey.BusinessObjects;

/// <summary>
/// Stores API Key information for a user.
/// The actual API Key is never stored - only its SHA256 hash.
/// </summary>
[DefaultClassOptions]
[NavigationItem("Security")]
[DefaultProperty(nameof(Description))]
[ImageName("BO_Security")]
public class ApiKeyInfo : BaseObject
{
    private string apiKeyHash = string.Empty;
    private Guid userOid;
    private DateTime? expiresAt;
    private DateTime createdAt;
    private DateTime? lastUsedAt;
    private bool isActive;
    private string description = string.Empty;

    public ApiKeyInfo(Session session) : base(session)
    {
    }

    public override void AfterConstruction()
    {
        base.AfterConstruction();
        createdAt = DateTime.UtcNow;
        isActive = true;
    }

    /// <summary>
    /// SHA256 hash of the API Key. The plaintext key is never stored.
    /// </summary>
    [Browsable(false)]
    [Size(64)]
    public string ApiKeyHash
    {
        get => apiKeyHash;
        set => SetPropertyValue(nameof(ApiKeyHash), ref apiKeyHash, value);
    }

    /// <summary>
    /// The Oid of the associated user (PermissionPolicyUser.Oid).
    /// </summary>
    [Indexed]
    public Guid UserOid
    {
        get => userOid;
        set => SetPropertyValue(nameof(UserOid), ref userOid, value);
    }

    /// <summary>
    /// Expiration date/time. Null means the key never expires.
    /// </summary>
    [ModelDefault("DisplayFormat", "{0:yyyy-MM-dd HH:mm:ss}")]
    [ModelDefault("EditMask", "yyyy-MM-dd HH:mm:ss")]
    public DateTime? ExpiresAt
    {
        get => expiresAt;
        set => SetPropertyValue(nameof(ExpiresAt), ref expiresAt, value);
    }

    /// <summary>
    /// When this API Key was created.
    /// </summary>
    [ModelDefault("DisplayFormat", "{0:yyyy-MM-dd HH:mm:ss}")]
    [ModelDefault("AllowEdit", "False")]
    public DateTime CreatedAt
    {
        get => createdAt;
        set => SetPropertyValue(nameof(CreatedAt), ref createdAt, value);
    }

    /// <summary>
    /// When this API Key was last used for authentication.
    /// </summary>
    [ModelDefault("DisplayFormat", "{0:yyyy-MM-dd HH:mm:ss}")]
    [ModelDefault("AllowEdit", "False")]
    public DateTime? LastUsedAt
    {
        get => lastUsedAt;
        set => SetPropertyValue(nameof(LastUsedAt), ref lastUsedAt, value);
    }

    /// <summary>
    /// Whether this API Key is currently active and can be used for authentication.
    /// </summary>
    public bool IsActive
    {
        get => isActive;
        set => SetPropertyValue(nameof(IsActive), ref isActive, value);
    }

    /// <summary>
    /// A description or name for this API Key to help identify its purpose.
    /// </summary>
    [Size(256)]
    public string Description
    {
        get => description;
        set => SetPropertyValue(nameof(Description), ref description, value);
    }

    /// <summary>
    /// Computed property to check if the key is expired.
    /// </summary>
    [VisibleInDetailView(false)]
    [VisibleInListView(true)]
    [PersistentAlias("Iif([ExpiresAt] Is Null, False, [ExpiresAt] < Now())")]
    public bool IsExpired
    {
        get
        {
            object? result = EvaluateAlias(nameof(IsExpired));
            return result != null && (bool)result;
        }
    }

    /// <summary>
    /// Computed property to check if the key is valid (active and not expired).
    /// </summary>
    [VisibleInDetailView(false)]
    [VisibleInListView(true)]
    public bool IsValid => IsActive && !IsExpired;
}
