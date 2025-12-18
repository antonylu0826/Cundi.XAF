using Cundi.XAF.FullTextSearch.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace Cundi.XAF.FullTextSearch.Controllers;

/// <summary>
/// Controller that handles navigation when a user clicks on a search result.
/// </summary>
public class GlobalSearchResultNavigationController : ObjectViewController<ListView, GlobalSearchResult>
{
    protected override void OnActivated()
    {
        base.OnActivated();

        DisableActions();

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

        base.OnDeactivated();
    }

    private void DisableActions()
    {
        var newController = Frame.GetController<NewObjectViewController>();
        if (newController != null)
        {
            newController.NewObjectAction.Active["GlobalSearchResult"] = false;
        }

        var deleteController = Frame.GetController<DeleteObjectsViewController>();
        if (deleteController != null)
        {
            deleteController.DeleteAction.Active["GlobalSearchResult"] = false;
        }

        var modifyController = Frame.GetController<ModificationsController>();
        if (modifyController != null)
        {
            modifyController.Active["GlobalSearchResult"] = false;
        }

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

        var objectSpace = Application.CreateObjectSpace(searchResult.TargetObjectType);
        var keyType = objectSpace.TypesInfo.FindTypeInfo(searchResult.TargetObjectType).KeyMember?.MemberType ?? typeof(object);
        var keyValue = ConvertKey(searchResult.TargetObjectKey, keyType);

        if (keyValue == null)
            return;

        var targetObject = objectSpace.GetObjectByKey(searchResult.TargetObjectType, keyValue);
        if (targetObject == null)
        {
            Application.ShowViewStrategy.ShowMessage("Object not found.", InformationType.Warning);
            return;
        }

        var detailView = Application.CreateDetailView(objectSpace, targetObject);
        var showViewParameters = new ShowViewParameters(detailView)
        {
            TargetWindow = TargetWindow.NewWindow
        };

        Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, null));
    }

    private static object? ConvertKey(string? keyString, Type keyType)
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

