using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;

namespace Cundi.XAF.ApiKey.Api;

/// <summary>
/// API module for ApiKey, provides Web API authentication support.
/// </summary>
public sealed class ApiKeyApiModule : ModuleBase
{
    public ApiKeyApiModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        RequiredModuleTypes.Add(typeof(ApiKeyModule));
    }

    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
    {
        return ModuleUpdater.EmptyModuleUpdaters;
    }

    public override void Setup(XafApplication application)
    {
        base.Setup(application);
    }

    public override void CustomizeTypesInfo(ITypesInfo typesInfo)
    {
        base.CustomizeTypesInfo(typesInfo);
    }
}
