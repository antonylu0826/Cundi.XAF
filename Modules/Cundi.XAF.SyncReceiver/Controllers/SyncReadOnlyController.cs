using Cundi.XAF.SyncReceiver.Attributes;
using Cundi.XAF.SyncReceiver.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;

namespace Cundi.XAF.SyncReceiver.Controllers;

/// <summary>
/// Controls the read-only behavior of sync objects in the UI.
/// Prevents editing of classes marked with SyncReadOnlyAttribute.
/// Also hides Delete button for SyncableObject-derived classes.
/// </summary>
public class SyncReadOnlyController : ViewController<DetailView>
{
    protected override void OnActivated()
    {
        base.OnActivated();

        var objectType = View.ObjectTypeInfo?.Type;
        if (objectType == null) return;

        // Check if the class is marked as SyncReadOnly
        var attr = objectType.GetCustomAttributes(typeof(SyncReadOnlyAttribute), true)
            .Cast<SyncReadOnlyAttribute>()
            .FirstOrDefault();

        if (attr?.IsReadOnly == true)
        {
            // Disable editing-related actions
            View.AllowEdit.SetItemValue("SyncReadOnly", false);
        }

        // Hide Delete button for SyncableObject-derived classes
        if (typeof(SyncableObject).IsAssignableFrom(objectType))
        {
            var deleteController = Frame.GetController<DeleteObjectsViewController>();
            if (deleteController != null)
            {
                deleteController.DeleteAction.Active.SetItemValue("SyncableObject", false);
            }
        }
    }

    protected override void OnDeactivated()
    {
        // Reset the edit restriction when leaving the view
        View.AllowEdit.RemoveItem("SyncReadOnly");

        var objectType = View.ObjectTypeInfo?.Type;
        if (objectType != null && typeof(SyncableObject).IsAssignableFrom(objectType))
        {
            var deleteController = Frame.GetController<DeleteObjectsViewController>();
            if (deleteController != null)
            {
                deleteController.DeleteAction.Active.RemoveItem("SyncableObject");
            }
        }

        base.OnDeactivated();
    }
}

/// <summary>
/// Hides the New button for SyncableObject-derived classes.
/// Also hides Delete button to prevent accidental deletion of synced data.
/// </summary>
public class SyncableObjectListViewController : ViewController<ListView>
{
    protected override void OnActivated()
    {
        base.OnActivated();

        var objectType = View.ObjectTypeInfo?.Type;
        if (objectType == null) return;

        // Check if the type inherits from SyncableObject
        if (typeof(SyncableObject).IsAssignableFrom(objectType))
        {
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
    }

    protected override void OnDeactivated()
    {
        var objectType = View.ObjectTypeInfo?.Type;
        if (objectType != null && typeof(SyncableObject).IsAssignableFrom(objectType))
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
        }

        base.OnDeactivated();
    }
}
