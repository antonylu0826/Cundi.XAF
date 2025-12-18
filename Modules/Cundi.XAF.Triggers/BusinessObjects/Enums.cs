namespace Cundi.XAF.Triggers.BusinessObjects;

/// <summary>
/// Trigger event types for object change detection.
/// </summary>
public enum TriggerEventType
{
    /// <summary>
    /// Triggered when a new object is created and saved.
    /// </summary>
    Created,

    /// <summary>
    /// Triggered when an existing object is modified and saved.
    /// </summary>
    Modified,

    /// <summary>
    /// Triggered when an object is deleted.
    /// </summary>
    Deleted
}

/// <summary>
/// HTTP methods for webhook requests.
/// </summary>
public enum HttpMethodType
{
    /// <summary>
    /// HTTP POST method (default for webhooks).
    /// </summary>
    Post,

    /// <summary>
    /// HTTP GET method.
    /// </summary>
    Get,

    /// <summary>
    /// HTTP PUT method.
    /// </summary>
    Put,

    /// <summary>
    /// HTTP PATCH method.
    /// </summary>
    Patch,

    /// <summary>
    /// HTTP DELETE method.
    /// </summary>
    Delete
}
