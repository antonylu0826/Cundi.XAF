using Cundi.XAF.Metadata.DatabaseUpdate;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace Cundi.XAF.Metadata
{
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
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
        }
    }
}
