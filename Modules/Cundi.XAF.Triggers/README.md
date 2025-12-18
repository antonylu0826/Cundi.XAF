# Cundi.XAF.Triggers

A DevExpress XAF module that detects object changes and triggers external webhooks based on configurable rules.

## Features

- **Object Change Detection**: Monitor XAF business objects for Create, Modify, and Delete operations
- **Trigger Rules**: Define rules to specify which object types and events should trigger webhooks
- **Flexible HTTP Methods**: Support for POST, GET, PUT, PATCH, and DELETE methods
- **Execution Logging**: Track all trigger executions with detailed status and response information  
- **Async Execution (WinForms/Blazor)**: Non-blocking webhook calls that don't impact UI performance
- **Sync Execution (WebApi)**: Ensures proper logging within request lifecycle
- **Hard Delete Mode**: Disables XPO deferred deletion (soft delete) globally
- **Clear Logs Action**: One-click action to clear all logs for a specific rule

## Architecture

The module supports two execution contexts:

| Context | Controller/Service | Execution Mode | Logging |
|---------|-------------------|----------------|---------|
| WinForms/Blazor | `TriggerWindowController` | Async (fire-and-forget) | Pre-execution log |
| WebApi | `WebApiMiddleDataService` | Sync | Full response log |

## Installation

### WinForms / Blazor Desktop

1. Add project reference to your XAF application:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\Modules\Cundi.XAF.Triggers\Cundi.XAF.Triggers.csproj" />
   </ItemGroup>
   ```

2. Add module to your application's `Module.cs`:
   ```csharp
   RequiredModuleTypes.Add(typeof(Cundi.XAF.Triggers.TriggersModule));
   ```

### WebApi

1. Add both project references:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\Modules\Cundi.XAF.Triggers\Cundi.XAF.Triggers.csproj" />
     <ProjectReference Include="..\Modules\Cundi.XAF.Triggers.Api\Cundi.XAF.Triggers.Api.csproj" />
   </ItemGroup>
   ```

2. Register the custom DataService in `Startup.cs`:
   ```csharp
   using Cundi.XAF.Triggers.Api;
   
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddScoped<IDataService, WebApiMiddleDataService>();
       // ... other services
   }
   ```

3. Update database schema by running your application.

## Usage

### Creating a Trigger Rule

1. Navigate to **Trigger Rules** in your XAF application
2. Create a new rule with:
   - **Name**: A descriptive name for the rule
   - **Target Type**: Select the object type to monitor from the dropdown
   - **OnCreated**: Check to trigger on object creation
   - **OnModified**: Check to trigger on object modification
   - **OnRemoved**: Check to trigger on object deletion
   - **HttpMethod**: Select HTTP method (default: POST)
   - **Webhook URL**: The endpoint to receive notifications
   - **Is Active**: Enable/disable the rule

### Testing Webhooks

In the TriggerRule detail view, click the **Test Webhook** button to send a test request to the configured URL. The test payload includes an `isTest: true` flag.

### Clearing Logs

In the TriggerRule detail view, click the **Clear Logs** button to delete all associated trigger logs for that rule.

### Log Retention Policy

Old trigger logs are automatically cleaned up on application startup. By default, logs older than **90 days** are deleted.

### Webhook Payload Format

When triggered, the module sends an HTTP request with the following JSON structure:

```json
{
  "eventType": "Created",
  "objectType": "YourNamespace.Customer",
  "objectKey": "abc123-def456",
  "timestamp": "2024-12-17T16:00:00.000Z",
  "triggerRule": "Customer Created Notification",
  "data": {
    "Name": "John Doe",
    "Email": "john@example.com"
  }
}
```

> **Note**: For `Deleted` events, the `data` field will be `null` as the object is no longer available.

### Viewing Trigger Logs

Navigate to **Trigger Logs** to see execution history including:
- Execution time
- HTTP method used
- Success/failure status
- HTTP status code
- Response body
- Error messages (if any)

## Configuration Options

### TriggerRule Properties

| Property | Type | Description |
|----------|------|-------------|
| `Name` | string | Rule name |
| `Description` | string | Optional description |
| `TargetType` | Type | Object type to monitor (dropdown selector) |
| `OnCreated` | bool | Trigger on object creation |
| `OnModified` | bool | Trigger on object modification |
| `OnRemoved` | bool | Trigger on object deletion |
| `IsActive` | bool | Enable/disable rule |
| `HttpMethod` | enum | HTTP method (Post, Get, Put, Patch, Delete) |
| `WebhookUrl` | string | Target webhook URL |
| `CustomHeaders` | string | JSON object with custom HTTP headers |

### Custom Headers Example

```json
{
  "Authorization": "Bearer your-token",
  "X-Custom-Header": "custom-value"
}
```

## Technical Notes

### Object Change Detection

The module uses XAF's ObjectSpace events to detect changes:

| Event Type | Detection Method |
|------------|------------------|
| Created | `IsNewObject()` in `Committing` event |
| Modified | Object in `ModifiedObjects` where not new and not deleted |
| Deleted | `GetObjectsToDelete()` in `Committing` event |

### Execution Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    ObjectSpace.Committing                    │
├─────────────────────────────────────────────────────────────┤
│  1. Capture object states (Created/Modified/Deleted)        │
│  2. Match against active TriggerRules                       │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    ObjectSpace.Committed                     │
├─────────────────────────────────────────────────────────────┤
│  3. Build webhook payload                                   │
│  4. Execute webhook (async for WinForms, sync for WebApi)   │
│  5. Log execution result                                    │
└─────────────────────────────────────────────────────────────┘
```

### Deferred Deletion

This module automatically disables XPO's deferred deletion (soft delete) feature. Objects will be permanently deleted from the database instead of being marked with a `GCRecord` timestamp.

## Requirements

- .NET 8.0+
- DevExpress XAF 24.2+

## License

MIT License
