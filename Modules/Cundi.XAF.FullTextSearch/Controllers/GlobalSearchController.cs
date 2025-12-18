using Cundi.XAF.FullTextSearch.BusinessObjects;
using Cundi.XAF.FullTextSearch.Services;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace Cundi.XAF.FullTextSearch.Controllers;

/// <summary>
/// Controller that provides global search functionality across all business objects.
/// Supports both Blazor and WinForms platforms with platform-specific UI placement.
/// </summary>
public class GlobalSearchController : WindowController
{
    private readonly ParametrizedAction _searchAction;

    public GlobalSearchController()
    {
        TargetWindowType = WindowType.Main;

        _searchAction = new ParametrizedAction(this, "GlobalSearch", PredefinedCategory.Tools, typeof(string))
        {
            Caption = "Global Search",
            ToolTip = "Search across all business objects",
            ImageName = "Action_Search"
        };

        _searchAction.Execute += SearchAction_Execute;
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        ConfigurePlatformSpecificSettings();
    }

    /// <summary>
    /// Configures action placement based on the current platform.
    /// </summary>
    private void ConfigurePlatformSpecificSettings()
    {
        if (Application == null) return;

        var appType = Application.GetType().FullName ?? "";

        if (appType.Contains("Blazor"))
        {
            // Blazor: Place action in header near user icon
            _searchAction.Category = "QuickAccess";
        }
        else
        {
            // WinForms: Place action in Tools toolbar
            _searchAction.Category = "Tools";
        }
    }

    private void SearchAction_Execute(object? sender, ParametrizedActionExecuteEventArgs e)
    {
        var keyword = e.ParameterCurrentValue?.ToString();
        if (string.IsNullOrWhiteSpace(keyword))
            return;

        ExecuteSearch(keyword);
    }

    private void ExecuteSearch(string keyword)
    {
        var nonPersistentOs = Application.CreateObjectSpace(typeof(GlobalSearchResult));

        // Get XPO ObjectSpace for querying persistent types
        var xpoObjectSpace = GetXpoObjectSpace();
        if (xpoObjectSpace == null)
        {
            Application.ShowViewStrategy.ShowMessage("Unable to create ObjectSpace for search.", InformationType.Error);
            return;
        }

        // Execute the search
        var searchService = new GlobalSearchService(xpoObjectSpace, Application.TypesInfo);
        var results = searchService.Search(keyword);

        if (results.Count == 0)
        {
            Application.ShowViewStrategy.ShowMessage($"No results found for '{keyword}'.", InformationType.Info);
            return;
        }

        // Create and populate result objects
        var resultObjects = CreateResultObjects(nonPersistentOs, results);

        // Show results in popup ListView
        ShowResultsView(nonPersistentOs, resultObjects);
    }

    private IObjectSpace? GetXpoObjectSpace()
    {
        foreach (var typeInfo in Application.TypesInfo.PersistentTypes.Where(t => t.IsPersistent && !t.IsAbstract))
        {
            try
            {
                return Application.CreateObjectSpace(typeInfo.Type);
            }
            catch { continue; }
        }
        return null;
    }

    private List<GlobalSearchResult> CreateResultObjects(IObjectSpace objectSpace, List<GlobalSearchResult> results)
    {
        var resultObjects = new List<GlobalSearchResult>();
        foreach (var result in results)
        {
            var npResult = objectSpace.CreateObject<GlobalSearchResult>();
            npResult.TargetObjectType = result.TargetObjectType;
            npResult.TargetObjectKey = result.TargetObjectKey;
            npResult.DisplayName = result.DisplayName;
            npResult.TypeCaption = result.TypeCaption;
            npResult.MatchedContent = result.MatchedContent;
            resultObjects.Add(npResult);
        }
        return resultObjects;
    }

    private void ShowResultsView(IObjectSpace objectSpace, List<GlobalSearchResult> resultObjects)
    {
        var listViewId = Application.FindListViewId(typeof(GlobalSearchResult));
        var collectionSource = Application.CreateCollectionSource(objectSpace, typeof(GlobalSearchResult), listViewId);

        foreach (var obj in resultObjects)
        {
            collectionSource.Add(obj);
        }

        var listView = Application.CreateListView(listViewId, collectionSource, false);

        var showViewParameters = new ShowViewParameters(listView)
        {
            TargetWindow = TargetWindow.NewModalWindow,
            Context = TemplateContext.PopupWindow
        };

        Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, _searchAction));
    }
}
