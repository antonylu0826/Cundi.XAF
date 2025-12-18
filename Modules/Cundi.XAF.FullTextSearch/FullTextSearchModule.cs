using Cundi.XAF.FullTextSearch.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

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

    public override void Setup(XafApplication application)
    {
        base.Setup(application);
    }

    public override void CustomizeTypesInfo(ITypesInfo typesInfo)
    {
        base.CustomizeTypesInfo(typesInfo);
        // GlobalSearchResult is already marked as non-persistent via [DomainComponent] attribute
    }
}
