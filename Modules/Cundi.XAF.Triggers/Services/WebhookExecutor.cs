using Cundi.XAF.Triggers.BusinessObjects;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using System.Text;
using System.Text.Json;

namespace Cundi.XAF.Triggers.Services;

/// <summary>
/// Executes webhook HTTP requests asynchronously.
/// </summary>
public class WebhookExecutor
{
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30) // Default timeout, will be overridden per request
    };

    private readonly IObjectSpaceFactory _objectSpaceFactory;

    public WebhookExecutor(IObjectSpaceFactory objectSpaceFactory)
    {
        _objectSpaceFactory = objectSpaceFactory;
    }

    /// <summary>
    /// Executes the webhook asynchronously without blocking (fire-and-forget).
    /// Logs execution BEFORE sending to ensure log is recorded on main thread.
    /// Note: Log status will show the trigger was fired, but response won't be recorded.
    /// </summary>
    public void ExecuteAsync(TriggerRule rule, string payload, string objectType, string objectKey, TriggerEventType eventType)
    {
        // Capture necessary data (avoid referencing XPO objects directly)
        var webhookUrl = rule.WebhookUrl;
        var httpMethod = rule.HttpMethod;
        var customHeaders = rule.CustomHeaders;
        var ruleOid = rule.Oid;

        // Log on main thread BEFORE firing webhook (to ensure log is created)
        try
        {
            LogExecutionByOid(ruleOid, objectType, objectKey, eventType, httpMethod, payload,
                true, null, CaptionHelper.GetLocalizedText(@"Messages\Cundi.XAF.Triggers", "WebhookFiredAsync"), null);
        }
        catch
        {
            // Swallow logging exceptions
        }

        // Fire and forget - webhook execution in background
        _ = Task.Run(async () =>
        {
            try
            {
                await SendRequestWithResultAsync(webhookUrl, httpMethod, customHeaders, payload);
            }
            catch
            {
                // Swallow exceptions in fire-and-forget mode
            }
        });
    }

    /// <summary>
    /// Executes the webhook synchronously and logs execution results.
    /// Use this in WebApi context where IServiceProvider is still available.
    /// </summary>
    public void ExecuteSync(TriggerRule rule, string payload, string objectType, string objectKey, TriggerEventType eventType)
    {
        // Capture necessary data (avoid referencing XPO objects directly)
        var webhookUrl = rule.WebhookUrl;
        var httpMethod = rule.HttpMethod;
        var customHeaders = rule.CustomHeaders;
        var ruleOid = rule.Oid;

        WebhookResult? result = null;
        try
        {
            // Execute synchronously to ensure ServiceProvider is available
            result = SendRequestWithResultAsync(webhookUrl, httpMethod, customHeaders, payload)
                .GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            result = new WebhookResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }

        // Log the execution result synchronously
        try
        {
            LogExecutionByOid(ruleOid, objectType, objectKey, eventType, httpMethod, payload,
                result?.IsSuccess ?? false, result?.StatusCode, result?.ResponseBody, result?.ErrorMessage);
        }
        catch
        {
            // Swallow logging exceptions
        }
    }

    /// <summary>
    /// Executes the webhook synchronously and logs the result.
    /// Use this for testing or when you need log records.
    /// </summary>
    public async Task<bool> ExecuteWithLoggingAsync(TriggerRule rule, string payload, string objectType, string objectKey, TriggerEventType eventType)
    {
        try
        {
            var result = await SendRequestAsync(rule, payload);

            // Log the result (must be called from main thread)
            LogExecution(rule, objectType, objectKey, eventType, payload,
                result.IsSuccess, result.StatusCode, result.ResponseBody, result.ErrorMessage);

            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            LogExecution(rule, objectType, objectKey, eventType, payload,
                false, null, null, ex.Message);
            return false;
        }
    }

    private async Task<WebhookResult> SendRequestWithResultAsync(string webhookUrl, HttpMethodType httpMethodType, string? customHeaders, string payload)
    {
        const int timeoutSeconds = 30;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

        try
        {
            var httpMethod = GetHttpMethod(httpMethodType);
            using var request = new HttpRequestMessage(httpMethod, webhookUrl);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

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

            var response = await _httpClient.SendAsync(request, cts.Token);
            var responseBody = await response.Content.ReadAsStringAsync();

            // Truncate response body if too long
            if (responseBody.Length > 4000)
            {
                responseBody = responseBody[..3997] + "...";
            }

            return new WebhookResult
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                ResponseBody = responseBody,
                ErrorMessage = response.IsSuccessStatusCode ? null : $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}"
            };
        }
        catch (TaskCanceledException)
        {
            return new WebhookResult
            {
                IsSuccess = false,
                ErrorMessage = CaptionHelper.GetLocalizedText(@"Messages\Cundi.XAF.Triggers", "RequestTimeout")
            };
        }
        catch (HttpRequestException ex)
        {
            return new WebhookResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private async Task<WebhookResult> SendRequestAsync(TriggerRule rule, string payload)
    {
        const int timeoutSeconds = 30; // Fixed timeout
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

        try
        {
            var httpMethod = GetHttpMethod(rule.HttpMethod);
            using var request = new HttpRequestMessage(httpMethod, rule.WebhookUrl);
            request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

            // Add custom headers
            if (!string.IsNullOrWhiteSpace(rule.CustomHeaders))
            {
                try
                {
                    var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(rule.CustomHeaders);
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

            var response = await _httpClient.SendAsync(request, cts.Token);
            var responseBody = await response.Content.ReadAsStringAsync(cts.Token);

            // Truncate response body if too long
            if (responseBody.Length > 4000)
            {
                responseBody = responseBody[..3997] + "...";
            }

            return new WebhookResult
            {
                IsSuccess = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                ResponseBody = responseBody,
                ErrorMessage = response.IsSuccessStatusCode ? null : $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}"
            };
        }
        catch (TaskCanceledException)
        {
            return new WebhookResult
            {
                IsSuccess = false,
                ErrorMessage = "Request timed out"
            };
        }
        catch (HttpRequestException ex)
        {
            return new WebhookResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private void LogExecution(
        TriggerRule rule,
        string objectType,
        string objectKey,
        TriggerEventType eventType,
        string payload,
        bool isSuccess,
        int? statusCode,
        string? responseBody,
        string? errorMessage)
    {
        try
        {
            using var objectSpace = _objectSpaceFactory.CreateObjectSpace<TriggerLog>();

            var log = objectSpace.CreateObject<TriggerLog>();
            log.TriggerRule = objectSpace.GetObject(rule);
            log.ExecutionTime = DateTime.UtcNow;
            log.ObjectType = objectType;
            log.ObjectKey = objectKey;
            log.EventType = eventType;
            log.HttpMethod = rule.HttpMethod;
            log.Payload = payload;
            log.IsSuccess = isSuccess;
            log.StatusCode = statusCode;
            log.ResponseBody = responseBody;
            log.ErrorMessage = errorMessage;

            objectSpace.CommitChanges();
        }
        catch
        {
            // Swallow logging exceptions to avoid affecting main flow
        }
    }

    private void LogExecutionByOid(
        Guid ruleOid,
        string objectType,
        string objectKey,
        TriggerEventType eventType,
        HttpMethodType httpMethod,
        string payload,
        bool isSuccess,
        int? statusCode,
        string? responseBody,
        string? errorMessage)
    {
        try
        {
            using var objectSpace = _objectSpaceFactory.CreateObjectSpace<TriggerLog>();

            // Find the rule by Oid
            var rule = objectSpace.GetObjectByKey<TriggerRule>(ruleOid);
            if (rule == null) return;

            var log = objectSpace.CreateObject<TriggerLog>();
            log.TriggerRule = rule;
            log.ExecutionTime = DateTime.UtcNow;
            log.ObjectType = objectType;
            log.ObjectKey = objectKey;
            log.EventType = eventType;
            log.HttpMethod = httpMethod;
            log.Payload = payload;
            log.IsSuccess = isSuccess;
            log.StatusCode = statusCode;
            log.ResponseBody = responseBody;
            log.ErrorMessage = errorMessage;

            objectSpace.CommitChanges();
        }
        catch
        {
            // Swallow logging exceptions to avoid affecting main flow
        }
    }

    private class WebhookResult
    {
        public bool IsSuccess { get; set; }
        public int? StatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public string? ErrorMessage { get; set; }
    }

    private static System.Net.Http.HttpMethod GetHttpMethod(HttpMethodType methodType)
    {
        return methodType switch
        {
            HttpMethodType.Get => System.Net.Http.HttpMethod.Get,
            HttpMethodType.Put => System.Net.Http.HttpMethod.Put,
            HttpMethodType.Patch => System.Net.Http.HttpMethod.Patch,
            HttpMethodType.Delete => System.Net.Http.HttpMethod.Delete,
            _ => System.Net.Http.HttpMethod.Post
        };
    }
}
