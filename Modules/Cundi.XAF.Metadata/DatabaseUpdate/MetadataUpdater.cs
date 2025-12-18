using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace Cundi.XAF.Metadata.DatabaseUpdate
{
    public class MetadataUpdater : ModuleUpdater
    {
        public MetadataUpdater(IObjectSpace objectSpace, Version currentDBVersion) :
            base(objectSpace, currentDBVersion)
        {
        }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

            // 在資料庫 schema 更新後自動掃描系統 metadata
            MetadataScanner.UpdateMetadata(ObjectSpace);

        }
    }
}
