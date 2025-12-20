using Cundi.XAF.DataMirror.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace Cundi.XAF.DataMirror.Controllers;

/// <summary>
/// Admin-only controller for MirrorTypeMappingConfig.
/// Only users with IsAdministrative role can view or modify mirror type mappings.
/// </summary>
public class MirrorTypeMappingAdminController : ViewController
{
    public MirrorTypeMappingAdminController()
    {
        TargetObjectType = typeof(MirrorTypeMappingConfig);
        TargetViewType = ViewType.Any;
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        // Check if current user is administrator
        bool isAdmin = IsCurrentUserAdministrator();

        if (!isAdmin)
        {
            // Disable view for non-admin users
            View.AllowEdit.SetItemValue("AdminOnly", false);
            View.AllowNew.SetItemValue("AdminOnly", false);
            View.AllowDelete.SetItemValue("AdminOnly", false);

            // Hide New action
            var newController = Frame.GetController<NewObjectViewController>();
            if (newController != null)
            {
                newController.NewObjectAction.Active.SetItemValue("AdminOnly", false);
            }

            // Hide Delete action
            var deleteController = Frame.GetController<DeleteObjectsViewController>();
            if (deleteController != null)
            {
                deleteController.DeleteAction.Active.SetItemValue("AdminOnly", false);
            }

            // Redirect to home or show message
            if (View is ListView)
            {
                // Hide navigation item would be handled by ShowNavigationItemController
            }
        }
    }

    protected override void OnDeactivated()
    {
        View.AllowEdit.RemoveItem("AdminOnly");
        View.AllowNew.RemoveItem("AdminOnly");
        View.AllowDelete.RemoveItem("AdminOnly");

        var newController = Frame.GetController<NewObjectViewController>();
        if (newController != null)
        {
            newController.NewObjectAction.Active.RemoveItem("AdminOnly");
        }

        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active.RemoveItem("AdminOnly");
        }

        base.OnDeactivated();
    }

    /// <summary>
    /// Checks if the currently logged-in user has an administrative role.
    /// </summary>
    private bool IsCurrentUserAdministrator()
    {
        try
        {
            var securitySystem = Application.GetSecurityStrategy();
            if (securitySystem?.User == null)
                return false;

            // Get the current user's roles
            var rolesProperty = securitySystem.User.GetType().GetProperty("Roles");
            if (rolesProperty == null)
                return false;

            var roles = rolesProperty.GetValue(securitySystem.User) as System.Collections.IEnumerable;
            if (roles == null)
                return false;

            foreach (var role in roles)
            {
                // Check if role is PermissionPolicyRole and IsAdministrative
                if (role is PermissionPolicyRole policyRole && policyRole.IsAdministrative)
                    return true;

                // Fallback: check IsAdministrative property via reflection
                var isAdminProperty = role.GetType().GetProperty("IsAdministrative");
                if (isAdminProperty != null)
                {
                    var isAdmin = isAdminProperty.GetValue(role);
                    if (isAdmin is bool adminValue && adminValue)
                        return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}

/// <summary>
/// Controls navigation visibility for MirrorTypeMappingConfig.
/// Only shows the navigation item to administrators.
/// </summary>
public class MirrorTypeMappingNavigationController : ShowNavigationItemController
{
    protected override void OnActivated()
    {
        base.OnActivated();
        CustomShowNavigationItem += MirrorTypeMappingNavigationController_CustomShowNavigationItem;
    }

    protected override void OnDeactivated()
    {
        CustomShowNavigationItem -= MirrorTypeMappingNavigationController_CustomShowNavigationItem;
        base.OnDeactivated();
    }

    private void MirrorTypeMappingNavigationController_CustomShowNavigationItem(object? sender, CustomShowNavigationItemEventArgs e)
    {
        // Check if navigating to MirrorTypeMappingConfig
        if (e.ActionArguments.SelectedChoiceActionItem?.Data is ViewShortcut viewShortcut)
        {
            // ViewShortcut.ObjectClass is a Type
            var objectType = viewShortcut.ObjectClass;

            if (objectType == typeof(MirrorTypeMappingConfig) ||
                (objectType != null && objectType.IsAssignableFrom(typeof(MirrorTypeMappingConfig))))
            {
                if (!IsCurrentUserAdministrator())
                {
                    e.Handled = true;
                    Application.ShowViewStrategy.ShowMessage(
                        "Access denied. Administrator privileges required.",
                        DevExpress.ExpressApp.InformationType.Error);
                }
            }
        }
    }

    private bool IsCurrentUserAdministrator()
    {
        try
        {
            var securitySystem = Application.GetSecurityStrategy();
            if (securitySystem?.User == null)
                return false;

            var rolesProperty = securitySystem.User.GetType().GetProperty("Roles");
            if (rolesProperty == null)
                return false;

            var roles = rolesProperty.GetValue(securitySystem.User) as System.Collections.IEnumerable;
            if (roles == null)
                return false;

            foreach (var role in roles)
            {
                if (role is PermissionPolicyRole policyRole && policyRole.IsAdministrative)
                    return true;

                var isAdminProperty = role.GetType().GetProperty("IsAdministrative");
                if (isAdminProperty != null)
                {
                    var isAdmin = isAdminProperty.GetValue(role);
                    if (isAdmin is bool adminValue && adminValue)
                        return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}
