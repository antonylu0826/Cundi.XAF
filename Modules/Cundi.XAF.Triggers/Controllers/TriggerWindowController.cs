using Cundi.XAF.Triggers.Services;
using DevExpress.ExpressApp;

namespace Cundi.XAF.Triggers.Controllers;

/// <summary>
/// WindowController that attaches trigger processing to ObjectSpace operations.
/// Automatically hooks into ObjectSpaceCreated events to enable trigger detection.
/// For WinForms/Blazor desktop applications.
/// </summary>
public class TriggerWindowController : WindowController
{
    /// <summary>
    /// Raised when a TriggerDataService is created for an ObjectSpace.
    /// </summary>
    public event EventHandler<DataServiceCreatedEventArgs>? DataServiceCreated;

    public TriggerWindowController()
    {
        TargetWindowType = WindowType.Main;
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        // Skip global event subscription if we are in a Web API context to avoid interference 
        // with WebApiMiddleDataService which handles its own triggers synchronously.
        if (Application.GetType().FullName?.Contains("WebApi") == true)
        {
            return;
        }
        Application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
    }

    protected override void OnDeactivated()
    {
        Application.ObjectSpaceCreated -= Application_ObjectSpaceCreated;
        base.OnDeactivated();
    }

    private void Application_ObjectSpaceCreated(object? sender, ObjectSpaceCreatedEventArgs e)
    {
        var dataService = new TriggerDataService();
        var serviceProvider = new DevExpress.ExpressApp.MiddleTier.ServiceProvider();
        serviceProvider.AddService(dataService);

        DataServiceCreated?.Invoke(this, new DataServiceCreatedEventArgs(serviceProvider, dataService));

        new TriggerCommitHelper(Application, dataService).Attach(e.ObjectSpace);
    }
}
