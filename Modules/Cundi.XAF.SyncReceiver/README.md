# Cundi.XAF.SyncReceiver Module

A DevExpress XAF module for receiving and processing data synchronization from webhooks. This module works as the destination endpoint for the `Cundi.XAF.Triggers` module's webhook functionality.

## Features

- **SyncableObject Base Class**: Custom XPO base class that allows external Oid assignment for primary key synchronization
- **Dynamic Type Mappings**: Configure source-to-local type mappings via XAF UI (SyncTypeMappingConfig), no code changes needed
- **Automatic Read-Only Protection**: All `SyncableObject` derived classes are automatically read-only in the UI
- **Auto-Hide New/Delete**: Automatically hides New and Delete buttons for all SyncableObject-derived classes
- **Sync Service**: Processes incoming webhook payloads and applies Create/Modify/Delete operations
- **Upsert Support**: Automatically creates objects on Modified events if they don't exist

## Installation

1. Add project reference to your XAF module project:

```xml
<ProjectReference Include="path\to\Cundi.XAF.SyncReceiver\Cundi.XAF.SyncReceiver.csproj" />
```

2. Register the module in your application:

```csharp
RequiredModuleTypes.Add(typeof(Cundi.XAF.SyncReceiver.SyncReceiverModule));
```

## Usage

### Creating a Syncable Business Object

Create a business object that inherits from `SyncableObject`:

```csharp
using Cundi.XAF.SyncReceiver.BusinessObjects;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;

namespace YourApp.Module.BusinessObjects;

[DefaultClassOptions]
public class SyncedCustomer : SyncableObject  // Automatically read-only in UI
{
    public SyncedCustomer(Session session) : base(session) { }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetPropertyValue(nameof(Name), ref _name, value);
    }
}
```

### Type Mappings

Type mappings are now configured dynamically via the XAF UI using `SyncTypeMappingConfig`:

1. Navigate to **Configuration > Sync Type Mapping Config** in your XAF application
2. Create a new mapping:
   - **Source Type Name**: The full type name from the source system (e.g., `Sample.Module.BusinessObjects.TriggerDemo`)
   - **Local Type**: Select from the dropdown showing all `SyncableObject` subclasses
   - **Is Active**: Enable the mapping

In your WebApi Startup.cs, simply register the services:

```csharp
using Cundi.XAF.SyncReceiver.Extensions;

// Register SyncReceiver services with a single call
services.AddSyncReceiver();
```

> **Note**: The type dropdown only shows classes that inherit from `SyncableObject`.

### SyncableObject Properties

| Property | Description |
|----------|-------------|
| `Oid` | Primary key that can be set externally for synchronization |
| `SyncedAt` | Timestamp of the last sync from the source system |

### Automatic UI Protection

All classes inheriting from `SyncableObject` automatically:
- **Read-only in DetailView** (data can only be modified via sync API)
- **Hide New button** in ListView (data can only be created via sync API)
- **Hide Delete button** in both ListView and DetailView (prevents accidental deletion)

## Webhook Payload Format

The module expects payloads in the format sent by `Cundi.XAF.Triggers`:

```json
{
  "eventType": "Created | Modified | Deleted",
  "objectType": "SourceNamespace.Customer",
  "objectKey": "guid-string",
  "timestamp": "2024-01-01T00:00:00.000Z",
  "triggerRule": "RuleName",
  "data": {
    "Name": "Customer Name",
    "Email": "customer@example.com"
  }
}
```

### Event Types

| Event | Behavior |
|-------|----------|
| `Created` | Creates a new object with the specified Oid |
| `Modified` | Updates existing object, or creates if not found (Upsert) |
| `Deleted` | Deletes the object (no error if already deleted) |

## Project Structure

```
Cundi.XAF.SyncReceiver/
├── BusinessObjects/
│   ├── SyncableObject.cs           # Base class for syncable objects
│   └── SyncTypeMappingConfig.cs    # Dynamic type mapping configuration
├── Controllers/
│   └── SyncReadOnlyController.cs   # UI protection controllers
├── DTOs/
│   └── SyncPayloadDto.cs           # Webhook payload structure
├── Services/
│   ├── SyncService.cs              # Sync processing logic
│   └── SyncTypeMappings.cs         # Type mapping service
├── TypeConverters/
│   └── SyncableTypeConverter.cs    # UI type selector for SyncableObject subclasses
└── SyncReceiverModule.cs           # Module definition
```

## Dependencies

- DevExpress.ExpressApp
- DevExpress.ExpressApp.Xpo
- DevExpress.Persistent.Base
- DevExpress.Persistent.BaseImpl.Xpo

## Related Modules

- [Cundi.XAF.SyncReceiver.Api](../Cundi.XAF.SyncReceiver.Api/README.md) - API endpoint for receiving webhook requests
- [Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README.md) - Source-side trigger module that sends webhooks
- [Cundi.XAF.ApiKey.Api](../Cundi.XAF.ApiKey.Api/README.md) - API Key authentication for the sync endpoint

## License

This project is licensed under the MIT License.
