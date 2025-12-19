# Cundi.XAF.SyncReceiver Module

A DevExpress XAF module for receiving and processing data synchronization from webhooks. This module works as the destination endpoint for the `Cundi.XAF.Triggers` module's webhook functionality.

## Features

- **SyncableObject Base Class**: Custom XPO base class that allows external Oid assignment for primary key synchronization
- **Type Mappings**: Map source system type names to local types using `SyncTypeMappings`
- **Read-Only Protection**: `SyncReadOnlyAttribute` and `SyncReadOnlyController` to prevent UI editing of synced objects
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
using Cundi.XAF.SyncReceiver.Attributes;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;

namespace YourApp.Module.BusinessObjects;

[DefaultClassOptions]
[SyncReadOnly] // Mark as read-only, can only be modified by sync API
public class SyncedCustomer : SyncableObject
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

When the source system uses different type names, configure mappings in your WebApi Startup:

```csharp
// Register SyncTypeMappings as singleton
services.AddSingleton<SyncTypeMappings>(sp =>
{
    var mappings = new SyncTypeMappings();
    // Map: Source.Module.Customer -> Local.Module.SyncedCustomer
    mappings.AddMapping<SyncedCustomer>("Source.Module.BusinessObjects.Customer");
    return mappings;
});

// Register SyncService as scoped
services.AddScoped<SyncService>();
```

### SyncableObject Properties

| Property | Description |
|----------|-------------|
| `Oid` | Primary key that can be set externally for synchronization |
| `SyncedAt` | Timestamp of the last sync from the source system |

### Automatic UI Protection

All classes inheriting from `SyncableObject` automatically:
- **Hide New button** in ListView (data can only be created via sync API)
- **Hide Delete button** in both ListView and DetailView (prevents accidental deletion)

### SyncReadOnlyAttribute

Use this attribute to additionally prevent UI editing:

```csharp
// On class level - entire object is read-only
[SyncReadOnly]
public class SyncedCustomer : SyncableObject { }

// Opt-out: Allow editing
[SyncReadOnly(false)]
public class EditableCustomer : SyncableObject { }
```

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
├── Attributes/
│   └── SyncReadOnlyAttribute.cs    # Read-only marker attribute
├── BusinessObjects/
│   └── SyncableObject.cs           # Base class for syncable objects
├── Controllers/
│   └── SyncReadOnlyController.cs   # UI protection controllers
├── DTOs/
│   └── SyncPayloadDto.cs           # Webhook payload structure
├── Services/
│   ├── SyncService.cs              # Sync processing logic
│   └── SyncTypeMappings.cs         # Type mapping configuration
└── SyncReceiverModule.cs           # Module definition
```

## Dependencies

- DevExpress.ExpressApp
- DevExpress.ExpressApp.Xpo
- DevExpress.Persistent.Base

## Related Modules

- [Cundi.XAF.SyncReceiver.Api](../Cundi.XAF.SyncReceiver.Api/README.md) - API endpoint for receiving webhook requests
- [Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README.md) - Source-side trigger module that sends webhooks
- [Cundi.XAF.ApiKey.Api](../Cundi.XAF.ApiKey.Api/README.md) - API Key authentication for the sync endpoint

## License

This project is licensed under the MIT License.
