using DevExpress.ExpressApp;

namespace Cundi.XAF.FullTextSearch;

/// <summary>
/// XAF module that provides global full-text search capabilities across all business objects.
/// </summary>
public sealed class FullTextSearchModule : ModuleBase
{
    public FullTextSearchModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
    }

}
