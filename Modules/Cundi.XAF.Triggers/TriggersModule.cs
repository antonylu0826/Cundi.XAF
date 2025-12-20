using Cundi.XAF.Triggers.Helpers;
using Cundi.XAF.Triggers.Services;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using Microsoft.Extensions.DependencyInjection;

namespace Cundi.XAF.Triggers;

/// <summary>
/// XAF module that provides object change detection and webhook triggering capabilities.
/// Trigger detection is handled by MiddleApplicationDataServiceController.
/// </summary>
public sealed class TriggersModule : ModuleBase
{
    private IObjectSpaceFactory? _objectSpaceFactory;

    public TriggersModule()
    {
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
        RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Validation.ValidationModule));
    }

    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB)
    {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }

    public override void Setup(XafApplication application)
    {
        base.Setup(application);
        DeferredDeletionHelper.DisableDeferredDeletion();

        // Subscribe to log cleanup after user logged on
        application.LoggedOn += Application_LoggedOn;
    }

    private void Application_LoggedOn(object? sender, LogonEventArgs e)
    {
        if (sender is not XafApplication application) return;

        // Get ObjectSpaceFactory for log cleanup
        _objectSpaceFactory ??= application.ServiceProvider?.GetService<IObjectSpaceFactory>();

        // Run log cleanup after user has logged on
        if (_objectSpaceFactory != null)
        {
            try
            {
                LogCleanupService.CleanupOldLogs(_objectSpaceFactory);
            }
            catch
            {
                // Swallow exceptions to avoid affecting application
            }
        }

        // Unsubscribe to run only once
        application.LoggedOn -= Application_LoggedOn;
    }

}
