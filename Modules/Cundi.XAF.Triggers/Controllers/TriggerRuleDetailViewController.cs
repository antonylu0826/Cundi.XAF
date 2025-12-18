using Cundi.XAF.Triggers.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
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
        // Clear Logs Action
        _clearLogsAction = new SimpleAction(this, "ClearTriggerLogs", PredefinedCategory.RecordEdit)
        {
            Caption = "Clear Logs",
            ImageName = "Action_Clear",
            ConfirmationMessage = "Are you sure you want to delete all trigger logs for this rule?",
            ToolTip = "Delete all trigger logs associated with this rule"
        };
        _clearLogsAction.Execute += ClearLogsAction_Execute;

        // Test Webhook Action
        _testWebhookAction = new SimpleAction(this, "TestWebhook", PredefinedCategory.RecordEdit)
        {
            Caption = "Test Webhook",
            ImageName = "Action_Debug_Start",
            ToolTip = "Send a test request to the webhook URL"
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
            Application.ShowViewStrategy.ShowMessage("No logs to clear.", InformationType.Info);
            return;
        }

        foreach (var log in logsToDelete)
        {
            ObjectSpace.Delete(log);
        }

        ObjectSpace.CommitChanges();

        Application.ShowViewStrategy.ShowMessage($"Cleared {logsToDelete.Count} log(s).", InformationType.Success);
    }

    private void TestWebhookAction_Execute(object? sender, SimpleActionExecuteEventArgs e)
    {
        var rule = ViewCurrentObject;
        if (rule == null) return;

        if (string.IsNullOrWhiteSpace(rule.WebhookUrl))
        {
            Application.ShowViewStrategy.ShowMessage("Webhook URL is required.", InformationType.Error);
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
                Application.ShowViewStrategy.ShowMessage(
                    $"✓ Test successful! HTTP {result.StatusCode}\n{result.ResponseBody}",
                    InformationType.Success);
            }
            else
            {
                Application.ShowViewStrategy.ShowMessage(
                    $"✗ Test failed! {result.ErrorMessage}",
                    InformationType.Warning);
            }
        }
        catch (Exception ex)
        {
            Application.ShowViewStrategy.ShowMessage(
                $"✗ Test failed!\n{ex.Message}",
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
