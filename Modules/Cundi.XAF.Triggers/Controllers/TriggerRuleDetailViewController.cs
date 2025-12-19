using Cundi.XAF.Triggers.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using System.Text;
using System.Text.Json;

namespace Cundi.XAF.Triggers.Controllers;

/// <summary>
/// View controller for TriggerRule DetailView that provides actions like clearing logs and testing webhooks.
/// </summary>
public class TriggerRuleDetailViewController : ObjectViewController<DetailView, TriggerRule>
{
    private readonly SimpleAction _clearLogsAction;
    private readonly SimpleAction _testWebhookAction;

    public TriggerRuleDetailViewController()
    {
        // Clear Logs Action - Caption, ToolTip, and ConfirmationMessage are defined in Model.DesignedDiffs.xafml
        _clearLogsAction = new SimpleAction(this, "ClearTriggerLogs", PredefinedCategory.RecordEdit)
        {
            ImageName = "Action_Clear"
        };
        _clearLogsAction.Execute += ClearLogsAction_Execute;

        // Test Webhook Action - Caption and ToolTip are defined in Model.DesignedDiffs.xafml
        _testWebhookAction = new SimpleAction(this, "TestWebhook", PredefinedCategory.RecordEdit)
        {
            ImageName = "Action_Debug_Start"
        };
        _testWebhookAction.Execute += TestWebhookAction_Execute;
    }

    private void ClearLogsAction_Execute(object? sender, SimpleActionExecuteEventArgs e)
    {
        var rule = ViewCurrentObject;
        if (rule == null) return;

        var logsToDelete = rule.TriggerLogs.ToList();

        if (logsToDelete.Count == 0)
        {
            Application.ShowViewStrategy.ShowMessage(
                CaptionHelper.GetLocalizedText(@"Messages\Cundi.XAF.Triggers", "NoLogsToClean"),
                InformationType.Info);
            return;
        }

        foreach (var log in logsToDelete)
        {
            ObjectSpace.Delete(log);
        }

        ObjectSpace.CommitChanges();

        Application.ShowViewStrategy.ShowMessage(
            CaptionHelper.GetLocalizedText(@"Messages\Cundi.XAF.Triggers", "ClearedLogsCount")
                .Replace("{Count}", logsToDelete.Count.ToString()),
            InformationType.Success);
    }

    private void TestWebhookAction_Execute(object? sender, SimpleActionExecuteEventArgs e)
    {
        var rule = ViewCurrentObject;
        if (rule == null) return;

        if (string.IsNullOrWhiteSpace(rule.WebhookUrl))
        {
            Application.ShowViewStrategy.ShowMessage(
                CaptionHelper.GetLocalizedText(@"Messages\Cundi.XAF.Triggers", "WebhookUrlRequired"),
                InformationType.Error);
            return;
        }

        // Capture values before async operation
        var webhookUrl = rule.WebhookUrl;
        var httpMethodType = rule.HttpMethod;
        var customHeaders = rule.CustomHeaders;
        var targetTypeName = rule.TargetType?.FullName ?? "TestObject";
        var ruleName = rule.Name;

        // Run synchronously to avoid cross-thread issues
        try
        {
            var result = SendTestRequest(webhookUrl, httpMethodType, customHeaders, targetTypeName, ruleName);

            if (result.IsSuccess)
            {
                var successMsg = CaptionHelper.GetLocalizedText(@"Messages\Cundi.XAF.Triggers", "TestSuccessful")
                    .Replace("{StatusCode}", result.StatusCode.ToString());
                Application.ShowViewStrategy.ShowMessage(
                    $"{successMsg}\n{result.ResponseBody}",
                    InformationType.Success);
            }
            else
            {
                Application.ShowViewStrategy.ShowMessage(
                    CaptionHelper.GetLocalizedText(@"Messages\Cundi.XAF.Triggers", "TestFailed")
                        .Replace("{ErrorMessage}", result.ErrorMessage ?? string.Empty),
                    InformationType.Warning);
            }
        }
        catch (Exception ex)
        {
            Application.ShowViewStrategy.ShowMessage(
                CaptionHelper.GetLocalizedText(@"Messages\Cundi.XAF.Triggers", "TestFailed")
                    .Replace("{ErrorMessage}", ex.Message),
                InformationType.Error);
        }
    }

    private static TestResult SendTestRequest(string webhookUrl, HttpMethodType httpMethodType, string? customHeaders, string targetTypeName, string ruleName)
    {
        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        // Build test payload
        var testPayload = new Dictionary<string, object?>
        {
            ["eventType"] = "Test",
            ["objectType"] = targetTypeName,
            ["objectKey"] = Guid.NewGuid().ToString(),
            ["timestamp"] = DateTime.UtcNow.ToString("o"),
            ["triggerRule"] = ruleName,
            ["isTest"] = true,
            ["data"] = new Dictionary<string, object>
            {
                ["message"] = "This is a test webhook request from Cundi.XAF.Triggers"
            }
        };

        var json = JsonSerializer.Serialize(testPayload, new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Determine HTTP method
        var httpMethod = httpMethodType switch
        {
            HttpMethodType.Get => HttpMethod.Get,
            HttpMethodType.Put => HttpMethod.Put,
            HttpMethodType.Patch => HttpMethod.Patch,
            HttpMethodType.Delete => HttpMethod.Delete,
            _ => HttpMethod.Post
        };

        using var request = new HttpRequestMessage(httpMethod, webhookUrl);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        // Add custom headers
        if (!string.IsNullOrWhiteSpace(customHeaders))
        {
            try
            {
                var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(customHeaders);
                if (headers != null)
                {
                    foreach (var (key, value) in headers)
                    {
                        request.Headers.TryAddWithoutValidation(key, value);
                    }
                }
            }
            catch
            {
                // Ignore invalid header JSON
            }
        }

        try
        {
            var response = httpClient.Send(request);
            using var reader = new StreamReader(response.Content.ReadAsStream());
            var responseBody = reader.ReadToEnd();

            // Truncate response if too long
            if (responseBody.Length > 500)
            {
                responseBody = responseBody[..497] + "...";
            }

            return new TestResult
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                ResponseBody = responseBody,
                ErrorMessage = response.IsSuccessStatusCode ? null : $"HTTP {(int)response.StatusCode}"
            };
        }
        catch (Exception ex)
        {
            return new TestResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private class TestResult
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
