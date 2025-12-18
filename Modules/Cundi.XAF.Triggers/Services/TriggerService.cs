using Cundi.XAF.Triggers.Helpers;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security.ClientServer;
using System.ComponentModel;

namespace Cundi.XAF.Triggers.Services;

/// <summary>
/// Service that handles trigger data operations during ObjectSpace commits.
/// </summary>
internal class TriggerDataService : IApplicationDataService
{
    public event EventHandler<DataServiceOperationEventArgs>? Committing;

    public void RaiseCommitting(DataServiceOperationEventArgs args)
    {
        Committing?.Invoke(this, args);
    }
}

/// <summary>
/// Event args for DataService creation.
/// </summary>
public class DataServiceCreatedEventArgs : EventArgs
{
    public DataServiceCreatedEventArgs(DevExpress.ExpressApp.MiddleTier.ServiceProvider serviceProvider, IApplicationDataService dataService)
    {
        DataService = dataService;
        ServiceProvider = serviceProvider;
    }
    public IApplicationDataService DataService { get; private set; }
    public DevExpress.ExpressApp.MiddleTier.IServiceProviderEx ServiceProvider { get; private set; }
}

/// <summary>
/// Event args for ObjectSpace operations containing modified objects.
/// </summary>
internal class ObjectSpaceDataServiceOperationEventArgs : DataServiceOperationEventArgs
{
    public System.Collections.IList? ModifiedObjects = null;

    public ObjectSpaceDataServiceOperationEventArgs(IObjectSpace objectSpace)
    {
        ObjectSpace = objectSpace;
    }

    public void RaiseCompleted()
    {
        OnCompleted();
    }
}

/// <summary>
/// Helper class that attaches to ObjectSpace commit events and invokes trigger processing.
/// </summary>
internal class TriggerCommitHelper
{
    private readonly XafApplication _application;
    private readonly TriggerDataService _dataService;
    private ObjectSpaceDataServiceOperationEventArgs? _dataServiceOperation;

    public DataServiceOperationEventArgs? DataServiceOperation => _dataServiceOperation;

    public TriggerCommitHelper(XafApplication application, TriggerDataService dataService)
    {
        _application = application;
        DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(dataService, nameof(dataService));
        _dataService = dataService;
    }

    public void Attach(IObjectSpace objectSpace)
    {
        objectSpace.Committing += ObjectSpace_Committing;
        objectSpace.Committed += ObjectSpace_Committed;
    }

    private void ObjectSpace_Committing(object? sender, CancelEventArgs e)
    {
        _dataServiceOperation = new ObjectSpaceDataServiceOperationEventArgs((IObjectSpace)sender!);
        _dataService.RaiseCommitting(_dataServiceOperation);

        // Combine ModifiedObjects and ObjectsToDelete to capture all changes
        var objectSpace = _dataServiceOperation.ObjectSpace;
        var allChangedObjects = new System.Collections.ArrayList();

        // Add modified objects (includes created and updated)
        foreach (var obj in objectSpace.ModifiedObjects)
        {
            allChangedObjects.Add(obj);
        }

        // Add objects to delete (may not be in ModifiedObjects)
        foreach (var obj in objectSpace.GetObjectsToDelete(true))
        {
            if (!allChangedObjects.Contains(obj))
            {
                allChangedObjects.Add(obj);
            }
        }

        _dataServiceOperation.ModifiedObjects = allChangedObjects;

        TriggerHelper.TriggeModifyingFlow(_application, _dataServiceOperation.ModifiedObjects);
    }

    private void ObjectSpace_Committed(object? sender, EventArgs e)
    {
        if (_dataServiceOperation != null)
        {
            if (_dataServiceOperation.ModifiedObjects != null)
            {
                TriggerHelper.TriggerModifiedFlow(_application, _dataServiceOperation.ModifiedObjects);
            }

            _dataServiceOperation.RaiseCompleted();
            _dataServiceOperation = null;
        }
    }
}
