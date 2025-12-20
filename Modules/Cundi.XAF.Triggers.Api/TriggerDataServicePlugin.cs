#nullable enable
using Cundi.XAF.Core.Api;
using Cundi.XAF.Triggers.BusinessObjects;
using Cundi.XAF.Triggers.Helpers;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;

namespace Cundi.XAF.Triggers.Api;

/// <summary>
/// DataService plugin that enables trigger processing for WebApi operations.
/// Intercepts ObjectSpace commit events to detect changes and invoke webhooks.
/// </summary>
public class TriggerDataServicePlugin : IDataServicePlugin
{
    private readonly IObjectSpaceFactory _objectSpaceFactory;
    private readonly IValidator _validator;

    /// <summary>
    /// Executes after other plugins (e.g., validation/blocking plugins should run first)
    /// </summary>
    public int Order => 100;

    public TriggerDataServicePlugin(
        IObjectSpaceFactory objectSpaceFactory,
        IValidator validator)
    {
        _objectSpaceFactory = objectSpaceFactory;
        _validator = validator;
    }

    public void OnObjectSpaceCreated(IObjectSpace objectSpace, Type objectType)
    {
        // Create a context object to track pending objects for this ObjectSpace
        var context = new TriggerContext(_objectSpaceFactory, _validator);
        objectSpace.Committing += context.Os_Committing;
        objectSpace.Committed += context.Os_Committed;
    }

    /// <summary>
    /// Internal context class to track pending objects per ObjectSpace instance
    /// </summary>
    private class TriggerContext
    {
        private readonly IObjectSpaceFactory _objectSpaceFactory;
        private readonly IValidator _validator;
        private Dictionary<object, TriggerEventType>? _pendingObjects = null;

        public TriggerContext(IObjectSpaceFactory objectSpaceFactory, IValidator validator)
        {
            _objectSpaceFactory = objectSpaceFactory;
            _validator = validator;
        }

        public void Os_Committing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (sender is not IObjectSpace os) return;

            // Validation
            var validationResult = _validator.RuleSet.ValidateAllTargets(
                os, os.ModifiedObjects, DefaultContexts.Save
            );
            if (validationResult.ValidationOutcome == ValidationOutcome.Error)
            {
                throw new ValidationException(validationResult);
            }

            // Combine ModifiedObjects and ObjectsToDelete to capture all changes
            var allChangedObjects = new System.Collections.ArrayList();

            // Add modified objects (includes created and updated)
            foreach (var obj in os.ModifiedObjects)
            {
                allChangedObjects.Add(obj);
            }

            // Add objects to delete (may not be in ModifiedObjects)
            foreach (var obj in os.GetObjectsToDelete(true))
            {
                if (!allChangedObjects.Contains(obj))
                {
                    allChangedObjects.Add(obj);
                }
            }

            // Capture states and store in instance field (Scoped service)
            _pendingObjects = TriggerHelper.CaptureObjectStates(allChangedObjects);
        }

        public void Os_Committed(object? sender, EventArgs e)
        {
            if (_pendingObjects != null && _pendingObjects.Count > 0)
            {
                using IObjectSpace nos = _objectSpaceFactory.CreateObjectSpace<TriggerRule>();
                TriggerHelper.TriggerModifiedFlow(null, nos, _objectSpaceFactory, _pendingObjects);
            }
            _pendingObjects = null;
        }
    }
}
