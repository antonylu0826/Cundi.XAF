using Cundi.XAF.SyncReceiver.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace Cundi.XAF.SyncReceiver.Controllers;

/// <summary>
/// Controls the read-only behavior in DetailView for SyncableObject-derived classes.
/// All classes inheriting from SyncableObject are read-only and cannot be edited in the UI.
/// </summary>
public class SyncReadOnlyController : ViewController<DetailView>
{
    public SyncReadOnlyController()
    {
        TargetObjectType = typeof(SyncableObject);
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        // Disable editing - all SyncableObject are read-only
        View.AllowEdit.SetItemValue("SyncableObject", false);

        // Hide Delete button
        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active.SetItemValue("SyncableObject", false);
        }
    }

    protected override void OnDeactivated()
    {
        View.AllowEdit.RemoveItem("SyncableObject");

        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active.RemoveItem("SyncableObject");
        }

        base.OnDeactivated();
    }
}

/// <summary>
/// Hides New and Delete buttons in ListView for SyncableObject-derived classes.
/// </summary>
public class SyncableObjectListViewController : ViewController<ListView>
{
    public SyncableObjectListViewController()
    {
        TargetObjectType = typeof(SyncableObject);
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        // Hide the New action
        var newObjectController = Frame.GetController<NewObjectViewController>();
        if (newObjectController != null)
        {
            newObjectController.NewObjectAction.Active.SetItemValue("SyncableObject", false);
        }

        // Hide the Delete action
        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active.SetItemValue("SyncableObject", false);
        }
    }

    protected override void OnDeactivated()
    {
        var newObjectController = Frame.GetController<NewObjectViewController>();
        if (newObjectController != null)
        {
            newObjectController.NewObjectAction.Active.RemoveItem("SyncableObject");
        }

        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active.RemoveItem("SyncableObject");
        }

        base.OnDeactivated();
    }
}
