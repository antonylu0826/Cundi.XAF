using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;

namespace Cundi.XAF.Triggers.Api;

/// <summary>
/// API module for Triggers, extends TriggersModule for WebApi context.
/// </summary>
public sealed class TriggersApiModule : ModuleBase
{
    public TriggersApiModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        RequiredModuleTypes.Add(typeof(TriggersModule));
    }

    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
    {
        return ModuleUpdater.EmptyModuleUpdaters;
    }

}
