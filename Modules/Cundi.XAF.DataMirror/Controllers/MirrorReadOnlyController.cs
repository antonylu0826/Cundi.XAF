using Cundi.XAF.DataMirror.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace Cundi.XAF.DataMirror.Controllers;

/// <summary>
/// Controls the read-only behavior in DetailView for MirroredObject-derived classes.
/// All classes inheriting from MirroredObject are read-only and cannot be edited in the UI.
/// </summary>
public class MirrorReadOnlyController : ViewController<DetailView>
{
    public MirrorReadOnlyController()
    {
        TargetObjectType = typeof(MirroredObject);
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        // Disable editing - all MirroredObject are read-only
        View.AllowEdit.SetItemValue("MirroredObject", false);

        // Hide Delete button
        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active.SetItemValue("MirroredObject", false);
        }
    }

    protected override void OnDeactivated()
    {
        View.AllowEdit.RemoveItem("MirroredObject");

        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active.RemoveItem("MirroredObject");
        }

        base.OnDeactivated();
    }
}

/// <summary>
/// Hides New and Delete buttons in ListView for MirroredObject-derived classes.
/// </summary>
public class MirroredObjectListViewController : ViewController<ListView>
{
    public MirroredObjectListViewController()
    {
        TargetObjectType = typeof(MirroredObject);
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        // Hide the New action
        var newObjectController = Frame.GetController<NewObjectViewController>();
        if (newObjectController != null)
        {
            newObjectController.NewObjectAction.Active.SetItemValue("MirroredObject", false);
        }

        // Hide the Delete action
        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active.SetItemValue("MirroredObject", false);
        }
    }

    protected override void OnDeactivated()
    {
        var newObjectController = Frame.GetController<NewObjectViewController>();
        if (newObjectController != null)
        {
            newObjectController.NewObjectAction.Active.RemoveItem("MirroredObject");
        }

        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active.RemoveItem("MirroredObject");
        }

        base.OnDeactivated();
    }
}
