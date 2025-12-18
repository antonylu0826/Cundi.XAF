using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

namespace Cundi.XAF.Metadata.Controllers
{
    public class MetadataController : ViewController
    {
        SimpleAction updateMetadataAction;
        public MetadataController()
        {
            updateMetadataAction = new SimpleAction(this, "UpdateMetadata", PredefinedCategory.Tools);
            updateMetadataAction.Caption = "Update Metadata";
            updateMetadataAction.ConfirmationMessage = "Are you sure you want to scan and update system metadata?";
            updateMetadataAction.Execute += UpdateMetadataAction_Execute;
        }

        private void UpdateMetadataAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            MetadataScanner.UpdateMetadata(Application.CreateObjectSpace(typeof(BusinessObjects.MetadataType)));
            Application.ShowViewStrategy.ShowMessage("Metadata updated successfully!", InformationType.Success);
        }
    }
}
