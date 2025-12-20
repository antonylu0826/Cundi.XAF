#nullable enable
using Cundi.XAF.Metadata.DatabaseUpdate;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace Cundi.XAF.Metadata;

/// <summary>
/// XAF module that provides metadata management capabilities.
/// Scans and stores type and property information for the application.
/// </summary>
public sealed class MetadataModule : ModuleBase
{
    public MetadataModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
    }

    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
    {
        ModuleUpdater updater = new MetadataUpdater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }
}
