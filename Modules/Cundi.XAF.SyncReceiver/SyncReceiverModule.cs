using DevExpress.ExpressApp;

namespace Cundi.XAF.SyncReceiver;

/// <summary>
/// XAF module for data synchronization receiver functionality.
/// Provides base classes and services for receiving and processing sync data from webhooks.
/// </summary>
public sealed class SyncReceiverModule : ModuleBase
{
    public SyncReceiverModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
    }

    public override void Setup(XafApplication application)
    {
        base.Setup(application);
    }
}
