using DevExpress.ExpressApp;

namespace Cundi.XAF.Metadata.Api
{
    public sealed class MetadataApiModule : ModuleBase
    {
        public MetadataApiModule()
        {
            RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
            RequiredModuleTypes.Add(typeof(Cundi.XAF.Metadata.MetadataModule));
        }
        public override void Setup(XafApplication application)
        {
            base.Setup(application);
        }
    }
}
