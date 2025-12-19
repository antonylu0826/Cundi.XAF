using System.Text.Json.Serialization;

namespace Cundi.XAF.SyncReceiver.DTOs;

/// <summary>
/// Data structure for sync webhook payload.
/// Corresponds to the format sent by Cundi.XAF.Triggers module.
/// </summary>
public class SyncPayloadDto
{
    /// <summary>
    /// Event type: Created, Modified, Deleted
    /// </summary>
    [JsonPropertyName("eventType")]
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Full type name of the object (FullName)
    /// </summary>
    [JsonPropertyName("objectType")]
    public string ObjectType { get; set; } = string.Empty;

    /// <summary>
    /// Object's primary key (Oid)
    /// </summary>
    [JsonPropertyName("objectKey")]
    public string ObjectKey { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the event occurred (UTC)
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = string.Empty;

    /// <summary>
    /// Name of the trigger rule
    /// </summary>
    [JsonPropertyName("triggerRule")]
    public string TriggerRule { get; set; } = string.Empty;

    /// <summary>
    /// Object's property data (Dictionary format)
    /// </summary>
    [JsonPropertyName("data")]
    public Dictionary<string, object?>? Data { get; set; }
}

/// <summary>
/// Sync event type enumeration
/// </summary>
public enum SyncEventType
{
    /// <summary>
    /// Object was created
    /// </summary>
    Created,

    /// <summary>
    /// Object was modified
    /// </summary>
    Modified,

    /// <summary>
    /// Object was deleted
    /// </summary>
    Deleted
}
