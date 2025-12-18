using Cundi.XAF.Triggers.BusinessObjects;
using Cundi.XAF.Triggers.Services;
using DevExpress.ExpressApp;
using DevExpress.Xpo;

namespace Cundi.XAF.Triggers.Helpers;

/// <summary>
/// Provides static helper methods for trigger processing.
/// Processes rules during object modifications and commits.
/// Supports both WinForms (via XafApplication) and WebApi (via IObjectSpace) contexts.
/// </summary>
public static class TriggerHelper
{
    // Thread-local storage for tracking object event types
    [ThreadStatic]
    private static Dictionary<object, TriggerEventType>? _pendingEventTypes;

    #region WinForms Mode (XafApplication)

    /// <summary>
    /// Called during ObjectSpace.Committing to capture object states before commit.
    /// For WinForms context using XafApplication.
    /// </summary>
    public static void TriggeModifyingFlow(XafApplication application, System.Collections.IList modifiedObjects)
    {
        _pendingEventTypes = CaptureObjectStates(modifiedObjects);
    }

    /// <summary>
    /// Called during ObjectSpace.Committed to process triggers after successful commit.
    /// For WinForms context using XafApplication. Uses async execution to avoid UI blocking.
    /// </summary>
    public static void TriggerModifiedFlow(XafApplication application, System.Collections.IList modifiedObjects)
    {
        if (_pendingEventTypes == null || _pendingEventTypes.Count == 0) return;

        var pendingCopy = new Dictionary<object, TriggerEventType>(_pendingEventTypes);
        _pendingEventTypes = null;

        try
        {
            // Get ObjectSpaceFactory from application
            var objectSpaceFactory = application?.ServiceProvider?.GetService(typeof(IObjectSpaceFactory)) as IObjectSpaceFactory;
            if (objectSpaceFactory == null) return;

            using var objectSpace = objectSpaceFactory.CreateObjectSpace<TriggerRule>();
            // Use async execution for WinForms to avoid UI blocking
            ProcessPendingObjects(objectSpace, objectSpaceFactory, pendingCopy, useSyncExecution: false);
        }
        catch
        {
            // Swallow exceptions to avoid affecting application
        }
    }

    #endregion

    #region WebApi Mode (IObjectSpace)

    /// <summary>
    /// Called during ObjectSpace.Committed to process triggers after successful commit.
    /// For WebApi context using IObjectSpace directly. Uses sync execution to ensure proper logging.
    /// </summary>
    public static void TriggerModifiedFlow(XafApplication? application, IObjectSpace objectSpace, IObjectSpaceFactory objectSpaceFactory, Dictionary<object, TriggerEventType> pendingObjects)
    {
        if (pendingObjects == null || pendingObjects.Count == 0) return;

        try
        {
            // Use sync execution for WebApi to ensure proper logging
            ProcessPendingObjects(objectSpace, objectSpaceFactory, pendingObjects, useSyncExecution: true);
        }
        catch
        {
            // Swallow exceptions to avoid affecting application
        }
    }

    #endregion

    #region Core Logic

    /// <summary>
    /// Captures the states of modified objects and returns a dictionary of event types.
    /// </summary>
    public static Dictionary<object, TriggerEventType> CaptureObjectStates(System.Collections.IList? modifiedObjects)
    {
        var pendingEventTypes = new Dictionary<object, TriggerEventType>();
        if (modifiedObjects == null || modifiedObjects.Count == 0) return pendingEventTypes;

        foreach (var obj in modifiedObjects)
        {
            if (obj == null) continue;

            // Skip trigger module's own objects to avoid recursion
            if (obj is TriggerRule || obj is TriggerLog) continue;

            // Determine event type based on object state
            TriggerEventType? eventType = null;

            if (obj is IObjectSpaceLink osLink && osLink.ObjectSpace != null)
            {
                var objectSpace = osLink.ObjectSpace;

                if (objectSpace.IsObjectToDelete(obj))
                {
                    eventType = TriggerEventType.Deleted;
                }
                else if (objectSpace.IsNewObject(obj))
                {
                    eventType = TriggerEventType.Created;
                }
                else
                {
                    eventType = TriggerEventType.Modified;
                }
            }
            else if (obj is XPBaseObject xpObj)
            {
                // Fallback for XPO objects
                if (xpObj.Session.IsObjectToDelete(xpObj))
                {
                    eventType = TriggerEventType.Deleted;
                }
                else if (xpObj.Session.IsNewObject(xpObj))
                {
                    eventType = TriggerEventType.Created;
                }
                else
                {
                    eventType = TriggerEventType.Modified;
                }
            }

            if (eventType.HasValue)
            {
                pendingEventTypes[obj] = eventType.Value;
            }
        }
        return pendingEventTypes;
    }

    private static void ProcessPendingObjects(IObjectSpace objectSpace, IObjectSpaceFactory? objectSpaceFactory, Dictionary<object, TriggerEventType> pendingObjects, bool useSyncExecution)
    {
        // Get active rules
        var activeRules = objectSpace.GetObjects<TriggerRule>()
            .Where(r => r.IsActive)
            .ToList();

        if (activeRules.Count == 0) return;

        // Need objectSpaceFactory for WebhookExecutor to create log entries
        if (objectSpaceFactory == null) return;

        var webhookExecutor = new WebhookExecutor(objectSpaceFactory);

        foreach (var (obj, eventType) in pendingObjects)
        {
            var objectTypeName = obj.GetType().FullName ?? obj.GetType().Name;
            var objectKey = GetObjectKey(obj);

            // Find matching rules based on event type
            var matchingRules = activeRules
                .Where(r => IsTypeMatch(r.TargetTypeName, objectTypeName) && IsEventTypeMatch(r, eventType))
                .ToList();

            foreach (var rule in matchingRules)
            {
                string payload;

                if (eventType == TriggerEventType.Deleted)
                {
                    payload = PayloadBuilder.BuildDeletedPayload(objectTypeName, objectKey, rule);
                }
                else
                {
                    payload = PayloadBuilder.BuildPayload(obj, eventType, rule, objectKey);
                }

                if (useSyncExecution)
                {
                    // Sync execution for WebApi - ensures logging works properly
                    webhookExecutor.ExecuteSync(rule, payload, objectTypeName, objectKey, eventType);
                }
                else
                {
                    // Async execution for WinForms - fire-and-forget to avoid UI blocking
                    webhookExecutor.ExecuteAsync(rule, payload, objectTypeName, objectKey, eventType);
                }
            }
        }
    }

    #endregion

    #region Helpers

    private static string GetObjectKey(object obj)
    {
        try
        {
            if (obj is XPBaseObject xpObj)
            {
                var keyProperty = xpObj.ClassInfo.KeyProperty;
                if (keyProperty != null)
                {
                    var keyValue = keyProperty.GetValue(xpObj);
                    return keyValue?.ToString() ?? string.Empty;
                }
            }

            // Try to find Oid property via reflection
            var oidProp = obj.GetType().GetProperty("Oid");
            if (oidProp != null)
            {
                var value = oidProp.GetValue(obj);
                return value?.ToString() ?? string.Empty;
            }
        }
        catch
        {
            // Ignore errors
        }
        return string.Empty;
    }

    private static bool IsTypeMatch(string? ruleTypeName, string objectTypeName)
    {
        if (string.IsNullOrEmpty(ruleTypeName)) return false;

        // Exact match
        if (string.Equals(ruleTypeName, objectTypeName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Check if rule type name is just the class name (without namespace)
        var objectClassName = objectTypeName.Split('.').LastOrDefault();
        if (string.Equals(ruleTypeName, objectClassName, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private static bool IsEventTypeMatch(TriggerRule rule, TriggerEventType eventType)
    {
        return eventType switch
        {
            TriggerEventType.Created => rule.OnCreated,
            TriggerEventType.Modified => rule.OnModified,
            TriggerEventType.Deleted => rule.OnRemoved,
            _ => false
        };
    }

    #endregion
}
