#nullable enable
using Cundi.XAF.Triggers.BusinessObjects;
using Cundi.XAF.Triggers.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.WebApi.Services;
using DevExpress.Persistent.Validation;

namespace Cundi.XAF.Triggers.Api;

/// <summary>
/// Custom DataService for WebApi that enables trigger processing.
/// Intercepts ObjectSpace commit events to detect changes and invoke webhooks.
/// </summary>
public class WebApiMiddleDataService : DataService
{
    private readonly IObjectSpaceFactory _objectSpaceFactory;
    private readonly IValidator _validator;
    private Dictionary<object, TriggerEventType>? _pendingObjects = null;

    public WebApiMiddleDataService(
        IObjectSpaceFactory objectSpaceFactory,
        ITypesInfo typesInfo,
        IValidator validator) : base(objectSpaceFactory, typesInfo)
    {
        _objectSpaceFactory = objectSpaceFactory;
        _validator = validator;
    }

    protected override IObjectSpace CreateObjectSpace(Type objectType)
    {
        var os = base.CreateObjectSpace(objectType);
        os.Committing += Os_Committing;
        os.Committed += Os_Committed;
        return os;
    }

    private void Os_Committing(object? sender, System.ComponentModel.CancelEventArgs e)
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

    private void Os_Committed(object? sender, EventArgs e)
    {
        if (_pendingObjects != null && _pendingObjects.Count > 0)
        {
            using IObjectSpace nos = _objectSpaceFactory.CreateObjectSpace<TriggerRule>();
            TriggerHelper.TriggerModifiedFlow(null, nos, _objectSpaceFactory, _pendingObjects);
        }
        _pendingObjects = null;
    }
}

