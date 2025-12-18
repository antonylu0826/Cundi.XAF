using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Cundi.XAF.Triggers.BusinessObjects;

/// <summary>
/// Defines a trigger rule that monitors specific object types and events,
/// then executes a webhook when conditions are met.
/// </summary>
[ImageName("Action_Inline_Edit")]
public class TriggerRule : BaseObject
{
    public TriggerRule(Session session) : base(session) { }

    public override void AfterConstruction()
    {
        base.AfterConstruction();
        IsActive = true;
        HttpMethod = HttpMethodType.Post;
    }

    private string _name = string.Empty;
    /// <summary>
    /// The name of the trigger rule.
    /// </summary>
    [Size(200)]
    [RuleRequiredField]
    public string Name
    {
        get => _name;
        set => SetPropertyValue(nameof(Name), ref _name, value);
    }

    private string? _description;
    /// <summary>
    /// Optional description for the trigger rule.
    /// </summary>
    [Size(500)]
    public string? Description
    {
        get => _description;
        set => SetPropertyValue(nameof(Description), ref _description, value);
    }

    private string? _TargetTypeName;
    [Browsable(false)]
    /// <summary>
    /// The full type name of the object to monitor (e.g., "YourNamespace.Customer").
    /// </summary>
    public string? TargetTypeName
    {
        get { return _TargetTypeName; }
        set
        {
            Type? type = value != null ? XafTypesInfo.Instance.FindTypeInfo(value)?.Type : null;
            if (_TargetType != type)
            {
                _TargetType = type;
            }
            SetPropertyValue<string?>(nameof(TargetTypeName), ref _TargetTypeName, value);
        }
    }

    private Type? _TargetType;
    [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
    [ImmediatePostData, NonPersistent]
    [RuleRequiredField]
    [VisibleInListView(false)]
    [Description("The .NET type of the target object associated with this trigger definition. This property is required and is updated automatically when the target object type name changes.")]
    public Type? TargetType
    {
        get { return _TargetType; }
        set
        {
            if (_TargetType != value)
            {
                _TargetType = value;
                TargetTypeName = value?.FullName;
            }
        }
    }

    private bool _onCreated;
    /// <summary>
    /// Trigger webhook when an object is created.
    /// </summary>
    public bool OnCreated
    {
        get => _onCreated;
        set => SetPropertyValue(nameof(OnCreated), ref _onCreated, value);
    }

    private bool _onModified;
    /// <summary>
    /// Trigger webhook when an object is modified.
    /// </summary>
    public bool OnModified
    {
        get => _onModified;
        set => SetPropertyValue(nameof(OnModified), ref _onModified, value);
    }

    private bool _onRemoved;
    /// <summary>
    /// Trigger webhook when an object is deleted/removed.
    /// </summary>
    public bool OnRemoved
    {
        get => _onRemoved;
        set => SetPropertyValue(nameof(OnRemoved), ref _onRemoved, value);
    }

    private bool _isActive;
    /// <summary>
    /// Whether the trigger rule is active. Inactive rules will not trigger webhooks.
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set => SetPropertyValue(nameof(IsActive), ref _isActive, value);
    }

    private HttpMethodType _httpMethod;
    /// <summary>
    /// The HTTP method to use when sending the webhook request.
    /// </summary>
    public HttpMethodType HttpMethod
    {
        get => _httpMethod;
        set => SetPropertyValue(nameof(HttpMethod), ref _httpMethod, value);
    }

    private string _webhookUrl = string.Empty;
    /// <summary>
    /// The URL to send the webhook request to.
    /// </summary>
    [Size(2000)]
    [RuleRequiredField]
    public string WebhookUrl
    {
        get => _webhookUrl;
        set => SetPropertyValue(nameof(WebhookUrl), ref _webhookUrl, value);
    }

    private string? _customHeaders;
    /// <summary>
    /// Custom HTTP headers in JSON format.
    /// Example: {"Authorization": "Bearer token", "X-Custom": "value"}
    /// </summary>
    [Size(SizeAttribute.Unlimited)]
    public string? CustomHeaders
    {
        get => _customHeaders;
        set => SetPropertyValue(nameof(CustomHeaders), ref _customHeaders, value);
    }

    /// <summary>
    /// Collection of trigger logs associated with this rule.
    /// </summary>
    [Association("TriggerRule-TriggerLogs")]
    [ModelDefault("AllowEdit", "False")]
    public XPCollection<TriggerLog> TriggerLogs => GetCollection<TriggerLog>(nameof(TriggerLogs));
}
