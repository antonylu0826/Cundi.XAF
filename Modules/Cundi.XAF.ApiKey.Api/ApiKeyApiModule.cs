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

}
