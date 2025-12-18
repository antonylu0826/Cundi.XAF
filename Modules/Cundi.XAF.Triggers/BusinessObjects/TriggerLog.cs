using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Cundi.XAF.Triggers.BusinessObjects;

/// <summary>
/// Records the execution history of trigger webhooks.
/// </summary>
[ImageName("Action_Debug_Start")]
public class TriggerLog : BaseObject
{
    public TriggerLog(Session session) : base(session) { }

    public override void AfterConstruction()
    {
        base.AfterConstruction();
        ExecutionTime = DateTime.UtcNow;
    }

    private TriggerRule? _triggerRule;
    /// <summary>
    /// The trigger rule that was executed.
    /// </summary>
    [Association("TriggerRule-TriggerLogs")]
    public TriggerRule? TriggerRule
    {
        get => _triggerRule;
        set => SetPropertyValue(nameof(TriggerRule), ref _triggerRule, value);
    }

    private DateTime _executionTime;
    /// <summary>
    /// The time when the webhook was executed.
    /// </summary>
    public DateTime ExecutionTime
    {
        get => _executionTime;
        set => SetPropertyValue(nameof(ExecutionTime), ref _executionTime, value);
    }

    private string _objectType = string.Empty;
    /// <summary>
    /// The type name of the object that triggered the webhook.
    /// </summary>
    [Size(500)]
    public string ObjectType
    {
        get => _objectType;
        set => SetPropertyValue(nameof(ObjectType), ref _objectType, value);
    }

    private string _objectKey = string.Empty;
    /// <summary>
    /// The primary key of the object that triggered the webhook.
    /// </summary>
    [Size(200)]
    public string ObjectKey
    {
        get => _objectKey;
        set => SetPropertyValue(nameof(ObjectKey), ref _objectKey, value);
    }

    private TriggerEventType _eventType;
    /// <summary>
    /// The type of event that triggered the webhook.
    /// </summary>
    public TriggerEventType EventType
    {
        get => _eventType;
        set => SetPropertyValue(nameof(EventType), ref _eventType, value);
    }

    private HttpMethodType _httpMethod;
    /// <summary>
    /// The HTTP method used for the webhook request.
    /// </summary>
    public HttpMethodType HttpMethod
    {
        get => _httpMethod;
        set => SetPropertyValue(nameof(HttpMethod), ref _httpMethod, value);
    }

    private bool _isSuccess;
    /// <summary>
    /// Whether the webhook execution was successful.
    /// </summary>
    public bool IsSuccess
    {
        get => _isSuccess;
        set => SetPropertyValue(nameof(IsSuccess), ref _isSuccess, value);
    }

    private int? _statusCode;
    /// <summary>
    /// The HTTP status code returned by the webhook endpoint.
    /// </summary>
    public int? StatusCode
    {
        get => _statusCode;
        set => SetPropertyValue(nameof(StatusCode), ref _statusCode, value);
    }

    private string? _responseBody;
    /// <summary>
    /// The response body from the webhook endpoint (truncated if too long).
    /// </summary>
    [Size(4000)]
    public string? ResponseBody
    {
        get => _responseBody;
        set => SetPropertyValue(nameof(ResponseBody), ref _responseBody, value);
    }

    private string? _errorMessage;
    /// <summary>
    /// Error message if the webhook execution failed.
    /// </summary>
    [Size(2000)]
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetPropertyValue(nameof(ErrorMessage), ref _errorMessage, value);
    }

    private string? _payload;
    /// <summary>
    /// The JSON payload that was sent to the webhook.
    /// </summary>
    [Size(SizeAttribute.Unlimited)]
    public string? Payload
    {
        get => _payload;
        set => SetPropertyValue(nameof(Payload), ref _payload, value);
    }
}
