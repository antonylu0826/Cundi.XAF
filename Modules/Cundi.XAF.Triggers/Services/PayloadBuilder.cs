using Cundi.XAF.Triggers.BusinessObjects;
using DevExpress.Xpo;
using System.Reflection;
using System.Text.Json;

namespace Cundi.XAF.Triggers.Services;

/// <summary>
/// Builds JSON payloads for webhook requests.
/// </summary>
public static class PayloadBuilder
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Builds a webhook payload for the given object and event.
    /// </summary>
    /// <param name="obj">The object that triggered the event.</param>
    /// <param name="eventType">The type of event.</param>
    /// <param name="rule">The trigger rule being executed.</param>
    /// <param name="objectKey">The primary key of the object.</param>
    /// <returns>JSON string payload.</returns>
    public static string BuildPayload(object obj, TriggerEventType eventType, TriggerRule rule, string objectKey)
    {
        var payload = new Dictionary<string, object?>
        {
            ["eventType"] = eventType.ToString(),
            ["objectType"] = obj.GetType().FullName,
            ["objectKey"] = objectKey,
            ["timestamp"] = DateTime.UtcNow.ToString("o"),
            ["triggerRule"] = rule.Name,
            ["data"] = ExtractObjectData(obj)
        };

        return JsonSerializer.Serialize(payload, _jsonOptions);
    }

    /// <summary>
    /// Builds a minimal payload for deleted objects (when full object data may not be available).
    /// </summary>
    public static string BuildDeletedPayload(string objectTypeName, string objectKey, TriggerRule rule)
    {
        var payload = new Dictionary<string, object?>
        {
            ["eventType"] = TriggerEventType.Deleted.ToString(),
            ["objectType"] = objectTypeName,
            ["objectKey"] = objectKey,
            ["timestamp"] = DateTime.UtcNow.ToString("o"),
            ["triggerRule"] = rule.Name,
            ["data"] = null
        };

        return JsonSerializer.Serialize(payload, _jsonOptions);
    }

    private static Dictionary<string, object?> ExtractObjectData(object obj)
    {
        var data = new Dictionary<string, object?>();
        var type = obj.GetType();

        // Get public properties with getters
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && !IsExcludedProperty(p));

        foreach (var prop in properties)
        {
            try
            {
                var value = prop.GetValue(obj);

                // Handle special types
                if (value == null)
                {
                    data[prop.Name] = null;
                }
                else if (IsSimpleType(prop.PropertyType))
                {
                    data[prop.Name] = value;
                }
                else if (value is DateTime dt)
                {
                    data[prop.Name] = dt.ToString("o");
                }
                else if (value is Guid guid)
                {
                    data[prop.Name] = guid.ToString();
                }
                else if (value is Enum enumValue)
                {
                    data[prop.Name] = enumValue.ToString();
                }
                else if (value is XPBaseObject xpObj)
                {
                    // For XPO objects, just include the key
                    data[prop.Name] = GetObjectKeyString(xpObj);
                }
                // Skip collections and complex objects to avoid circular references
            }
            catch
            {
                // Skip properties that throw exceptions
            }
        }

        return data;
    }

    private static bool IsExcludedProperty(PropertyInfo prop)
    {
        // Exclude XPO internal properties
        var excludedNames = new[]
        {
            "Session", "ClassInfo", "IsLoading", "IsDeleted",
            "OptimisticLockField", "OptimisticLockFieldInDataLayer",
            "GCRecord", "Loading", "Oid"
        };

        return excludedNames.Contains(prop.Name);
    }

    private static bool IsSimpleType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        return underlyingType.IsPrimitive
            || underlyingType == typeof(string)
            || underlyingType == typeof(decimal)
            || underlyingType == typeof(DateTime)
            || underlyingType == typeof(Guid)
            || underlyingType.IsEnum;
    }

    private static string GetObjectKeyString(XPBaseObject obj)
    {
        try
        {
            var keyProperty = obj.ClassInfo.KeyProperty;
            if (keyProperty != null)
            {
                var keyValue = keyProperty.GetValue(obj);
                return keyValue?.ToString() ?? string.Empty;
            }
        }
        catch
        {
            // Ignore errors
        }
        return string.Empty;
    }
}
