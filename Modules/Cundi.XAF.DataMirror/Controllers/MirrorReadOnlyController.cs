#nullable enable
using Cundi.XAF.DataMirror.Attributes;
using Cundi.XAF.DataMirror.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace Cundi.XAF.DataMirror.Controllers;

/// <summary>
/// Controls the read-only behavior in DetailView for MirroredObject-derived classes.
/// Only classes marked with [MirroredObjectProtection(true)] are read-only.
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

        // Only apply protection if the type is marked as protected
        if (View.CurrentObject != null &&
            MirroredObjectProtectionAttribute.IsTypeProtected(View.CurrentObject.GetType()))
        {
            // Disable editing
            View.AllowEdit.SetItemValue("MirroredObjectProtection", false);

            // Hide Delete button
            var deleteController = Frame.GetController<DeleteObjectsViewController>();
            if (deleteController != null)
            {
                deleteController.DeleteAction.Active.SetItemValue("MirroredObjectProtection", false);
            }
        }
    }

    protected override void OnDeactivated()
    {
        View.AllowEdit.RemoveItem("MirroredObjectProtection");

        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active.RemoveItem("MirroredObjectProtection");
        }

        base.OnDeactivated();
    }
}

/// <summary>
/// Hides New and Delete buttons in ListView for MirroredObject-derived classes.
/// Only classes marked with [MirroredObjectProtection(true)] are protected.
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

        // Only apply protection if the type is marked as protected
        if (View.ObjectTypeInfo?.Type != null &&
            MirroredObjectProtectionAttribute.IsTypeProtected(View.ObjectTypeInfo.Type))
        {
            // Hide the New action
            var newObjectController = Frame.GetController<NewObjectViewController>();
            if (newObjectController != null)
            {
                newObjectController.NewObjectAction.Active.SetItemValue("MirroredObjectProtection", false);
            }

            // Hide the Delete action
            var deleteController = Frame.GetController<DeleteObjectsViewController>();
            if (deleteController != null)
            {
                deleteController.DeleteAction.Active.SetItemValue("MirroredObjectProtection", false);
            }
        }
    }

    protected override void OnDeactivated()
    {
        var newObjectController = Frame.GetController<NewObjectViewController>();
        if (newObjectController != null)
        {
            newObjectController.NewObjectAction.Active.RemoveItem("MirroredObjectProtection");
        }

        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active.RemoveItem("MirroredObjectProtection");
        }

        base.OnDeactivated();
    }
}
