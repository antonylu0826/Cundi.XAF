using DevExpress.ExpressApp;

namespace Cundi.XAF.SyncReceiver.Api;

/// <summary>
/// XAF API module for data synchronization receiver.
/// Provides REST API endpoints for receiving sync data from webhooks.
/// </summary>
public sealed class SyncReceiverApiModule : ModuleBase
{
    public SyncReceiverApiModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        RequiredModuleTypes.Add(typeof(SyncReceiver.SyncReceiverModule));
    }

    public override void Setup(XafApplication application)
    {
        base.Setup(application);
    }
}
