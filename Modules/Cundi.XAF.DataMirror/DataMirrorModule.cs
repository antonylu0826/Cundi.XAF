using DevExpress.ExpressApp;

namespace Cundi.XAF.DataMirror;

/// <summary>
/// XAF module for data mirror functionality.
/// Provides base classes and services for receiving and processing mirrored data from webhooks.
/// </summary>
public sealed class DataMirrorModule : ModuleBase
{
    public DataMirrorModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
    }

    public override void Setup(XafApplication application)
    {
        base.Setup(application);
    }
}
