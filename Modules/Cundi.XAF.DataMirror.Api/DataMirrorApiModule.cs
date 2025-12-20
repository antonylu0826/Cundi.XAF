using DevExpress.ExpressApp;

namespace Cundi.XAF.DataMirror.Api;

/// <summary>
/// XAF API module for data mirror receiver.
/// Provides REST API endpoints for receiving mirrored data from webhooks.
/// </summary>
public sealed class DataMirrorApiModule : ModuleBase
{
    public DataMirrorApiModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        RequiredModuleTypes.Add(typeof(DataMirror.DataMirrorModule));
    }

    public override void Setup(XafApplication application)
    {
        base.Setup(application);
    }
}
