using Cundi.XAF.FullTextSearch.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

namespace Cundi.XAF.FullTextSearch.Controllers;

/// <summary>
/// Controller that handles navigation when a user clicks on a search result.
/// </summary>
public class GlobalSearchResultNavigationController : ObjectViewController<ListView, GlobalSearchResult>
{
    public GlobalSearchResultNavigationController()
    {
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        // Disable unnecessary actions
        DisableActions();

        // Handle double-click to navigate
        View.ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;

        // Subscribe to ListViewProcessCurrentObjectController for double-click handling
        var processController = Frame.GetController<ListViewProcessCurrentObjectController>();
        if (processController != null)
        {
            processController.CustomProcessSelectedItem += ProcessController_CustomProcessSelectedItem;
        }
    }

    protected override void OnDeactivated()
    {
        var processController = Frame.GetController<ListViewProcessCurrentObjectController>();
        if (processController != null)
        {
            processController.CustomProcessSelectedItem -= ProcessController_CustomProcessSelectedItem;
        }

        View.ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
        base.OnDeactivated();
    }

    private void ObjectSpace_ObjectChanged(object? sender, ObjectChangedEventArgs e)
    {
        // Not needed for non-persistent objects
    }

    private void DisableActions()
    {
        // Disable New action
        var newController = Frame.GetController<NewObjectViewController>();
        if (newController != null)
        {
            newController.NewObjectAction.Active["GlobalSearchResult"] = false;
        }

        // Disable Delete action
        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active["GlobalSearchResult"] = false;
        }

        // Disable Edit action (pencil icon)
        var modifyController = Frame.GetController<ModificationsController>();
        if (modifyController != null)
        {
            modifyController.Active["GlobalSearchResult"] = false;
        }

        // Disable selection column if possible
        if (View is ListView listView)
        {
            listView.Editor.AllowEdit = false;
        }
    }

    private void ProcessController_CustomProcessSelectedItem(object? sender, CustomProcessListViewSelectedItemEventArgs e)
    {
        var searchResult = e.InnerArgs.CurrentObject as GlobalSearchResult;
        if (searchResult?.TargetObjectType == null || string.IsNullOrEmpty(searchResult.TargetObjectKey))
            return;

        e.Handled = true;
        NavigateToObject(searchResult);
    }

    private void NavigateToObject(GlobalSearchResult searchResult)
    {
        if (searchResult.TargetObjectType == null)
            return;

        Console.WriteLine($"[GlobalSearch] Navigating to {searchResult.TargetObjectType.Name} with key {searchResult.TargetObjectKey}");

        // Create an object space for the target type
        var objectSpace = Application.CreateObjectSpace(searchResult.TargetObjectType);

        // Get the key type and parse the key value
        var keyType = objectSpace.TypesInfo.FindTypeInfo(searchResult.TargetObjectType).KeyMember?.MemberType ?? typeof(object);
        object? keyValue = ConvertKey(searchResult.TargetObjectKey, keyType);

        if (keyValue == null)
        {
            Console.WriteLine($"[GlobalSearch] Failed to convert key: {searchResult.TargetObjectKey}");
            return;
        }

        // Get the target object
        var targetObject = objectSpace.GetObjectByKey(searchResult.TargetObjectType, keyValue);
        if (targetObject == null)
        {
            Console.WriteLine($"[GlobalSearch] Object not found with key: {keyValue}");
            Application.ShowViewStrategy.ShowMessage($"Object not found.", InformationType.Warning);
            return;
        }

        // Show the DetailView in a new window
        var detailView = Application.CreateDetailView(objectSpace, targetObject);
        var showViewParameters = new ShowViewParameters(detailView)
        {
            TargetWindow = TargetWindow.NewWindow
        };

        Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, null));
    }

    /// <summary>
    /// Converts the string key to the appropriate type.
    /// </summary>
    private object? ConvertKey(string? keyString, Type keyType)
    {
        if (string.IsNullOrEmpty(keyString))
            return null;

        try
        {
            if (keyType == typeof(Guid))
                return Guid.Parse(keyString);
            if (keyType == typeof(int))
                return int.Parse(keyString);
            if (keyType == typeof(long))
                return long.Parse(keyString);
            if (keyType == typeof(string))
                return keyString;

            return Convert.ChangeType(keyString, keyType);
        }
        catch
        {
            return null;
        }
    }
}
