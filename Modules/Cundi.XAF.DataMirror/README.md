# Cundi.XAF.DataMirror Module

A DevExpress XAF module for receiving and processing mirrored data from webhooks. This module works as the destination endpoint for the `Cundi.XAF.Triggers` module's webhook functionality.

## Features

- **MirroredObject Base Class**: Custom XPO base class that allows external Oid assignment for primary key synchronization
- **Protection Attribute**: `[MirroredObjectProtection]` attribute to control whether objects are read-only or editable
- **Dynamic Type Mappings**: Configure source-to-local type mappings via XAF UI (MirrorTypeMappingConfig), no code changes needed
- **Configurable Read-Only Protection**: Only `MirroredObject` derived classes marked with `[MirroredObjectProtection(true)]` are read-only
- **Auto-Hide New/Delete**: Automatically hides New and Delete buttons for protected MirroredObject-derived classes
- **Mirror Service**: Processes incoming webhook payloads and applies Create/Modify/Delete operations
- **Upsert Support**: Automatically creates objects on Modified events if they don't exist
- **Admin-Only Configuration**: Type mapping configuration is restricted to administrators only

## Installation

1. Add project reference to your XAF module project:

```xml
<ProjectReference Include="path\to\Cundi.XAF.DataMirror\Cundi.XAF.DataMirror.csproj" />
```

2. Register the module in your application:

```csharp
RequiredModuleTypes.Add(typeof(Cundi.XAF.DataMirror.DataMirrorModule));
```

## Usage

### Creating a Mirrored Business Object

Create a business object that inherits from `MirroredObject`:

```csharp
using Cundi.XAF.DataMirror.Attributes;
using Cundi.XAF.DataMirror.BusinessObjects;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;

namespace YourApp.Module.BusinessObjects;

// Protected: Read-only in both UI and API
[DefaultClassOptions]
[MirroredObjectProtection(true)]
public class SyncedCustomer : MirroredObject
{
    public SyncedCustomer(Session session) : base(session) { }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetPropertyValue(nameof(Name), ref _name, value);
    }
}

// Editable: Can be modified in UI and API (default behavior)
[DefaultClassOptions]
public class EditableCustomer : MirroredObject
{
    public EditableCustomer(Session session) : base(session) { }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetPropertyValue(nameof(Name), ref _name, value);
    }
}
```

### Type Mappings

Type mappings are configured dynamically via the XAF UI using `MirrorTypeMappingConfig`:

1. Navigate to **Configuration > Mirror Type Mapping** in your XAF application
2. Create a new mapping:
   - **Source Type Name**: The full type name from the source system (e.g., `Sample.Module.BusinessObjects.TriggerDemo`)
   - **Local Type**: Select from the dropdown showing all `MirroredObject` subclasses
   - **Is Active**: Enable the mapping

> **Note**: The type dropdown only shows classes that inherit from `MirroredObject`.
> **Note**: Type mapping configuration is restricted to administrator users only.

### MirroredObject Properties

| Property | Description |
|----------|-------------|
| `Oid` | Primary key that can be set externally for synchronization |
| `SyncedAt` | Timestamp of the last sync from the source system |

### Protection Attribute

The `[MirroredObjectProtection]` attribute controls whether a `MirroredObject` derived class is protected from modifications:

| Setting | UI Behavior | API Behavior |
|---------|-------------|---------------|
| `[MirroredObjectProtection(true)]` | Read-only, no New/Delete buttons | POST/PUT/DELETE blocked |
| No attribute (default) | Fully editable | All operations allowed |

> **Note**: By default, `MirroredObject` derived classes are **editable**. Add `[MirroredObjectProtection(true)]` to protect objects from modifications.

### Automatic UI Protection

Classes marked with `[MirroredObjectProtection(true)]` automatically:
- **Read-only in DetailView** (data can only be modified via mirror API)
- **Hide New button** in ListView (data can only be created via mirror API)
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
Cundi.XAF.DataMirror/
├── Attributes/
│   └── MirroredObjectProtectionAttribute.cs  # Controls object protection behavior
├── BusinessObjects/
│   ├── MirroredObject.cs              # Base class for mirrored objects
│   └── MirrorTypeMappingConfig.cs     # Dynamic type mapping configuration
├── Controllers/
│   ├── MirrorReadOnlyController.cs    # UI protection controllers
│   └── MirrorTypeMappingAdminController.cs  # Admin-only access control
├── DTOs/
│   └── MirrorPayloadDto.cs            # Webhook payload structure
├── Services/
│   ├── MirrorService.cs               # Mirror processing logic
│   └── MirrorTypeMappings.cs          # Type mapping service
├── TypeConverters/
│   └── MirroredTypeConverter.cs       # UI type selector for MirroredObject subclasses
└── DataMirrorModule.cs                # Module definition
```

## Dependencies

- DevExpress.ExpressApp
- DevExpress.ExpressApp.Xpo
- DevExpress.ExpressApp.Security
- DevExpress.Persistent.Base
- DevExpress.Persistent.BaseImpl.Xpo

## Related Modules

- [Cundi.XAF.DataMirror.Api](../Cundi.XAF.DataMirror.Api/README.md) - API endpoint for receiving webhook requests
- [Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README.md) - Source-side trigger module that sends webhooks
- [Cundi.XAF.ApiKey.Api](../Cundi.XAF.ApiKey.Api/README.md) - API Key authentication for the mirror endpoint

## License

This project is licensed under the MIT License.
