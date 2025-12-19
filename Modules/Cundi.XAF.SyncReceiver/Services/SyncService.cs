using Cundi.XAF.SyncReceiver.BusinessObjects;
using Cundi.XAF.SyncReceiver.DTOs;
using DevExpress.ExpressApp;
using System.Text.Json;

namespace Cundi.XAF.SyncReceiver.Services;

/// <summary>
/// Sync service for processing webhook sync requests.
/// </summary>
public class SyncService
{
    private readonly IObjectSpaceFactory _objectSpaceFactory;
    private readonly SyncTypeMappings _typeMappings;

    public SyncService(IObjectSpaceFactory objectSpaceFactory, SyncTypeMappings typeMappings)
    {
        _objectSpaceFactory = objectSpaceFactory;
        _typeMappings = typeMappings;
    }

    /// <summary>
    /// Processes a sync payload from the webhook.
    /// </summary>
    /// <param name="payload">The sync payload to process.</param>
    /// <returns>The result of the sync operation.</returns>
    public SyncResult ProcessSync(SyncPayloadDto payload)
    {
        // Parse event type
        if (!Enum.TryParse<SyncEventType>(payload.EventType, true, out var eventType))
        {
            return SyncResult.Failure($"Invalid event type: {payload.EventType}");
        }

        // Parse object type (check type mappings first, then fall back to XafTypesInfo)
        Type? objectType = null;
        if (_typeMappings.TryGetMappedType(payload.ObjectType, out var mappedType))
        {
            objectType = mappedType;
        }
        else
        {
            objectType = XafTypesInfo.Instance.FindTypeInfo(payload.ObjectType)?.Type;
        }

        if (objectType == null)
        {
            return SyncResult.Failure($"Unknown object type: {payload.ObjectType}. Add a type mapping using AddTypeMapping() method.");
        }

        // Validate that the object type inherits from SyncableObject
        if (!typeof(SyncableObject).IsAssignableFrom(objectType))
        {
            return SyncResult.Failure($"Type {payload.ObjectType} does not inherit from SyncableObject and is not syncable.");
        }

        // Parse ObjectKey (Oid)
        if (!Guid.TryParse(payload.ObjectKey, out var oid))
        {
            return SyncResult.Failure($"Invalid object key (must be a valid GUID): {payload.ObjectKey}");
        }

        // Execute the corresponding operation based on event type
        try
        {
            return eventType switch
            {
                SyncEventType.Created => HandleCreated(objectType, oid, payload.Data),
                SyncEventType.Modified => HandleModified(objectType, oid, payload.Data),
                SyncEventType.Deleted => HandleDeleted(objectType, oid),
                _ => SyncResult.Failure($"Unsupported event type: {eventType}")
            };
        }
        catch (Exception ex)
        {
            return SyncResult.Failure($"Error processing sync: {ex.Message}");
        }
    }

    private SyncResult HandleCreated(Type objectType, Guid oid, Dictionary<string, object?>? data)
    {
        using var objectSpace = _objectSpaceFactory.CreateObjectSpace(objectType);

        // Check if the object already exists
        var existing = objectSpace.GetObjectByKey(objectType, oid);
        if (existing != null)
        {
            return SyncResult.Failure($"Object with Oid {oid} already exists. Use 'Modified' event to update.");
        }

        // Create new object
        var obj = (SyncableObject)objectSpace.CreateObject(objectType);
        obj.Oid = oid;
        obj.SyncedAt = DateTime.UtcNow;

        // Set property values
        if (data != null)
        {
            SetProperties(objectSpace, obj, data);
        }

        objectSpace.CommitChanges();
        return SyncResult.Success($"Successfully created object with Oid {oid}");
    }

    private SyncResult HandleModified(Type objectType, Guid oid, Dictionary<string, object?>? data)
    {
        using var objectSpace = _objectSpaceFactory.CreateObjectSpace(objectType);

        var obj = objectSpace.GetObjectByKey(objectType, oid) as SyncableObject;
        if (obj == null)
        {
            // Object doesn't exist, create it (Upsert behavior)
            objectSpace.Dispose();
            return HandleCreated(objectType, oid, data);
        }

        // Update property values
        if (data != null)
        {
            SetProperties(objectSpace, obj, data);
        }
        obj.SyncedAt = DateTime.UtcNow;

        objectSpace.CommitChanges();
        return SyncResult.Success($"Successfully modified object with Oid {oid}");
    }

    private SyncResult HandleDeleted(Type objectType, Guid oid)
    {
        using var objectSpace = _objectSpaceFactory.CreateObjectSpace(objectType);

        var obj = objectSpace.GetObjectByKey(objectType, oid);
        if (obj == null)
        {
            return SyncResult.Success($"Object with Oid {oid} not found (already deleted or never existed).");
        }

        objectSpace.Delete(obj);
        objectSpace.CommitChanges();
        return SyncResult.Success($"Successfully deleted object with Oid {oid}");
    }

    private void SetProperties(IObjectSpace objectSpace, object obj, Dictionary<string, object?> data)
    {
        var typeInfo = XafTypesInfo.Instance.FindTypeInfo(obj.GetType());

        foreach (var (propertyName, value) in data)
        {
            var memberInfo = typeInfo.FindMember(propertyName);
            if (memberInfo == null || !memberInfo.IsPublic) continue;

            // Skip Oid and SyncedAt (controlled by the system)
            if (propertyName == "Oid" || propertyName == "SyncedAt") continue;

            // Skip non-persistent or read-only members
            if (!memberInfo.IsPersistent) continue;

            try
            {
                var convertedValue = ConvertValue(objectSpace, memberInfo.MemberType, value);
                memberInfo.SetValue(obj, convertedValue);
            }
            catch
            {
                // Ignore properties that cannot be set
            }
        }
    }

    private object? ConvertValue(IObjectSpace objectSpace, Type targetType, object? value)
    {
        if (value == null) return null;

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // Handle JsonElement (from System.Text.Json deserialization)
        if (value is JsonElement jsonElement)
        {
            return ConvertJsonElement(objectSpace, targetType, jsonElement);
        }

        // Handle Guid
        if (underlyingType == typeof(Guid) && value is string guidStr)
        {
            return Guid.TryParse(guidStr, out var guid) ? guid : null;
        }

        // Handle DateTime
        if (underlyingType == typeof(DateTime) && value is string dateStr)
        {
            return DateTime.TryParse(dateStr, out var dt) ? dt : null;
        }

        // Handle Enum
        if (underlyingType.IsEnum && value is string enumStr)
        {
            return Enum.TryParse(underlyingType, enumStr, true, out var enumVal) ? enumVal : null;
        }

        // Handle XPO reference objects (query by Oid)
        if (typeof(SyncableObject).IsAssignableFrom(underlyingType) && value is string refOidStr)
        {
            if (Guid.TryParse(refOidStr, out var refOid))
            {
                return objectSpace.GetObjectByKey(underlyingType, refOid);
            }
        }

        // General type conversion
        try
        {
            return Convert.ChangeType(value, underlyingType);
        }
        catch
        {
            return null;
        }
    }

    private object? ConvertJsonElement(IObjectSpace objectSpace, Type targetType, JsonElement element)
    {
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        return element.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.String => ConvertValue(objectSpace, targetType, element.GetString()),
            JsonValueKind.Number when underlyingType == typeof(int) => element.GetInt32(),
            JsonValueKind.Number when underlyingType == typeof(long) => element.GetInt64(),
            JsonValueKind.Number when underlyingType == typeof(decimal) => element.GetDecimal(),
            JsonValueKind.Number when underlyingType == typeof(double) => element.GetDouble(),
            JsonValueKind.Number when underlyingType == typeof(float) => element.GetSingle(),
            JsonValueKind.Number when underlyingType == typeof(short) => (short)element.GetInt32(),
            JsonValueKind.Number when underlyingType == typeof(byte) => (byte)element.GetInt32(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => element.GetRawText()
        };
    }
}

/// <summary>
/// Result of a sync operation.
/// </summary>
public class SyncResult
{
    /// <summary>
    /// Whether the sync operation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Message describing the result of the operation.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Creates a successful result with the specified message.
    /// </summary>
    public static SyncResult Success(string message) => new() { IsSuccess = true, Message = message };

    /// <summary>
    /// Creates a failure result with the specified message.
    /// </summary>
    public static SyncResult Failure(string message) => new() { IsSuccess = false, Message = message };
}
