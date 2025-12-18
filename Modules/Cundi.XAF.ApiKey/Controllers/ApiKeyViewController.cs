using Cundi.XAF.ApiKey.BusinessObjects;
using Cundi.XAF.ApiKey.Parameters;
using Cundi.XAF.ApiKey.Services;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace Cundi.XAF.ApiKey.Controllers;

/// <summary>
/// ViewController for managing API Keys on user objects.
/// Provides actions to generate and revoke API keys.
/// Only visible to administrators in DetailView.
/// </summary>
public class ApiKeyViewController : ObjectViewController<DetailView, ISecurityUser>
{
    private readonly PopupWindowShowAction generateApiKeyAction;
    private readonly SimpleAction revokeApiKeyAction;

    public ApiKeyViewController()
    {
        // Generate API Key action with popup dialog
        generateApiKeyAction = new PopupWindowShowAction(this, "GenerateApiKey", PredefinedCategory.RecordEdit)
        {
            Caption = "Generate API Key",
            ImageName = "Action_Grant",
            ToolTip = "Generate a new API key for this user. Any existing key will be replaced.",
            SelectionDependencyType = SelectionDependencyType.RequireSingleObject
        };
        generateApiKeyAction.CustomizePopupWindowParams += GenerateApiKeyAction_CustomizePopupWindowParams;
        generateApiKeyAction.Execute += GenerateApiKeyAction_Execute;

        // Revoke API Key action
        revokeApiKeyAction = new SimpleAction(this, "RevokeApiKey", PredefinedCategory.RecordEdit)
        {
            Caption = "Revoke API Key",
            ImageName = "Action_Deny",
            ToolTip = "Revoke the API key for this user.",
            SelectionDependencyType = SelectionDependencyType.RequireSingleObject,
            ConfirmationMessage = "This will permanently revoke the API Key for the selected user. Continue?"
        };
        revokeApiKeyAction.Execute += RevokeApiKeyAction_Execute;
    }

    protected override void OnActivated()
    {
        base.OnActivated();
        View.CurrentObjectChanged += View_CurrentObjectChanged;
        UpdateActionState();
    }

    protected override void OnDeactivated()
    {
        View.CurrentObjectChanged -= View_CurrentObjectChanged;
        base.OnDeactivated();
    }

    private void View_CurrentObjectChanged(object? sender, EventArgs e)
    {
        UpdateActionState();
    }

    private void UpdateActionState()
    {
        // Check if current user is administrator
        bool isAdmin = IsCurrentUserAdministrator();

        var user = View.CurrentObject as ISecurityUser;
        bool hasValidUser = user != null && isAdmin;

        if (!hasValidUser)
        {
            generateApiKeyAction.Active.SetItemValue("IsAdmin", isAdmin);
            revokeApiKeyAction.Active.SetItemValue("IsAdmin", isAdmin);
            generateApiKeyAction.Enabled.SetItemValue("HasUser", false);
            revokeApiKeyAction.Enabled.SetItemValue("HasUser", false);
            return;
        }

        generateApiKeyAction.Active.SetItemValue("IsAdmin", true);
        revokeApiKeyAction.Active.SetItemValue("IsAdmin", true);
        generateApiKeyAction.Enabled.SetItemValue("HasUser", true);

        // Check if user has an existing API key
        var userOid = GetUserOid(user!);
        if (userOid.HasValue)
        {
            var existingKey = ObjectSpace.FindObject<ApiKeyInfo>(
                DevExpress.Data.Filtering.CriteriaOperator.Parse("UserOid = ?", userOid.Value));
            revokeApiKeyAction.Enabled.SetItemValue("HasApiKey", existingKey != null);
        }
        else
        {
            revokeApiKeyAction.Enabled.SetItemValue("HasApiKey", false);
        }
    }

    /// <summary>
    /// Checks if the currently logged-in user has an administrative role.
    /// </summary>
    private bool IsCurrentUserAdministrator()
    {
        try
        {
            var securitySystem = Application.GetSecurityStrategy();
            if (securitySystem?.User == null)
                return false;

            // Get the current user's roles
            var rolesProperty = securitySystem.User.GetType().GetProperty("Roles");
            if (rolesProperty == null)
                return false;

            var roles = rolesProperty.GetValue(securitySystem.User) as System.Collections.IEnumerable;
            if (roles == null)
                return false;

            foreach (var role in roles)
            {
                // Check if role is PermissionPolicyRole and IsAdministrative
                if (role is PermissionPolicyRole policyRole && policyRole.IsAdministrative)
                    return true;

                // Fallback: check IsAdministrative property via reflection
                var isAdminProperty = role.GetType().GetProperty("IsAdministrative");
                if (isAdminProperty != null)
                {
                    var isAdmin = isAdminProperty.GetValue(role);
                    if (isAdmin is bool adminValue && adminValue)
                        return true;
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    private void GenerateApiKeyAction_CustomizePopupWindowParams(object? sender, CustomizePopupWindowParamsEventArgs e)
    {
        var objectSpace = Application.CreateObjectSpace(typeof(ApiKeyGenerationParameters));
        var parameters = objectSpace.CreateObject<ApiKeyGenerationParameters>();

        e.View = Application.CreateDetailView(objectSpace, parameters);
        e.DialogController.SaveOnAccept = false;
    }

    private void GenerateApiKeyAction_Execute(object? sender, PopupWindowShowActionExecuteEventArgs e)
    {
        var user = View.CurrentObject as ISecurityUser;
        if (user == null) return;

        var userOid = GetUserOid(user);
        if (!userOid.HasValue)
        {
            Application.ShowViewStrategy.ShowMessage("Unable to get user identifier.", InformationType.Error);
            return;
        }

        // Get parameters from popup
        var parameters = (ApiKeyGenerationParameters)e.PopupWindowViewCurrentObject;

        // Check for existing API key and remove it
        var existingKey = ObjectSpace.FindObject<ApiKeyInfo>(
            DevExpress.Data.Filtering.CriteriaOperator.Parse("UserOid = ?", userOid.Value));
        if (existingKey != null)
        {
            ObjectSpace.Delete(existingKey);
        }

        // Generate new API key
        var (plaintextKey, hashValue) = ApiKeyGenerator.Generate();

        // Create new ApiKeyInfo with expiration
        var apiKeyInfo = ObjectSpace.CreateObject<ApiKeyInfo>();
        apiKeyInfo.UserOid = userOid.Value;
        apiKeyInfo.ApiKeyHash = hashValue;
        apiKeyInfo.Description = $"API Key for {user.UserName}";
        apiKeyInfo.ExpiresAt = parameters.GetExpirationDate();

        ObjectSpace.CommitChanges();

        // Show the plaintext key to the user with expiration info
        ShowApiKeyDialog(plaintextKey, user.UserName, apiKeyInfo.ExpiresAt);

        UpdateActionState();
    }

    private void RevokeApiKeyAction_Execute(object? sender, SimpleActionExecuteEventArgs e)
    {
        var user = View.CurrentObject as ISecurityUser;
        if (user == null) return;

        var userOid = GetUserOid(user);
        if (!userOid.HasValue) return;

        var existingKey = ObjectSpace.FindObject<ApiKeyInfo>(
            DevExpress.Data.Filtering.CriteriaOperator.Parse("UserOid = ?", userOid.Value));
        if (existingKey != null)
        {
            ObjectSpace.Delete(existingKey);
            ObjectSpace.CommitChanges();
            Application.ShowViewStrategy.ShowMessage(
                $"API Key for user '{user.UserName}' has been revoked.",
                InformationType.Success);
        }

        UpdateActionState();
    }

    private Guid? GetUserOid(ISecurityUser user)
    {
        // Try to get Oid from the user object using reflection
        var oidProperty = user.GetType().GetProperty("Oid");
        if (oidProperty != null)
        {
            var value = oidProperty.GetValue(user);
            if (value is Guid guid)
                return guid;
        }
        return null;
    }

    private void ShowApiKeyDialog(string plaintextKey, string userName, DateTime? expiresAt)
    {
        // Try to copy to clipboard first
        try
        {
            CopyToClipboard(plaintextKey);
        }
        catch { /* Ignore clipboard errors */ }

        // Build expiration info
        string expirationInfo = expiresAt.HasValue
            ? expiresAt.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
            : "Never";

        // Create result display object
        var objectSpace = Application.CreateObjectSpace(typeof(ApiKeyResultDisplay));
        var result = objectSpace.CreateObject<ApiKeyResultDisplay>();
        result.ApiKey = plaintextKey;
        result.UserName = userName;
        result.ExpiresAt = expirationInfo;
        result.Warning = "⚠️ IMPORTANT: Copy this key now! It will NOT be shown again. The key has been copied to clipboard.";

        // Show in popup DetailView that user must manually close
        var detailView = Application.CreateDetailView(objectSpace, result, false);
        detailView.Caption = "API Key Generated Successfully";

        var showViewParameters = new ShowViewParameters(detailView)
        {
            TargetWindow = TargetWindow.NewModalWindow,
            Context = TemplateContext.PopupWindow
        };

        Application.ShowViewStrategy.ShowView(showViewParameters, new ShowViewSource(Frame, null));
    }

    private static void CopyToClipboard(string text)
    {
        // Platform-agnostic clipboard copy using PowerShell on Windows
        if (OperatingSystem.IsWindows())
        {
            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "powershell";
            process.StartInfo.Arguments = $"-command \"Set-Clipboard -Value '{text.Replace("'", "''")}'\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }
    }
}
