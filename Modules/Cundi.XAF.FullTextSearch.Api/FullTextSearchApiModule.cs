using DevExpress.ExpressApp;

namespace Cundi.XAF.FullTextSearch.Api;

/// <summary>
/// XAF module that provides Web API endpoints for global full-text search.
/// </summary>
public sealed class FullTextSearchApiModule : ModuleBase
{
    public FullTextSearchApiModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        RequiredModuleTypes.Add(typeof(FullTextSearchModule));
    }
}

