using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace Cundi.XAF.ApiKey.DatabaseUpdate;

/// <summary>
/// Database updater for ApiKey module.
/// </summary>
public class Updater : ModuleUpdater
{
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion)
    {
    }

    public override void UpdateDatabaseAfterUpdateSchema()
    {
        base.UpdateDatabaseAfterUpdateSchema();
    }

    public override void UpdateDatabaseBeforeUpdateSchema()
    {
        base.UpdateDatabaseBeforeUpdateSchema();
    }
}
