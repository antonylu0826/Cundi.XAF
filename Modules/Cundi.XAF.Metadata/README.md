# Cundi.XAF.Metadata

**Cundi.XAF.Metadata** is an XAF module designed to scan and record Persistent Object information within the system.
It automatically synchronizes Business Object types and their properties to the database for use by other modules (such as Reporting, Security, or API).

## Features

- **Auto-Scanning**: Automatically scans all types marked as `Persistent` and `Visible` when the database schema is updated.
- **Data Persistence**:
  - `MetadataType`: Records the full name and assembly name of object types.
  - `MetadataProperty`: Records object properties, including raw type names (Full Name) and friendly type names (e.g., `List<String>`).
- **System Maintenance Protection**: All Metadata data is **Read-Only** and **Non-Creatable** in the UI to ensure data consistency and accuracy.
- **Manual Update Mechanism**: Provide an Action for administrators to manually trigger Metadata updates at runtime.

## Installation

Add this module to your XAF project's (Blazor, WinForms, or WebApi) `Module.cs` or `Startup.cs`:

```csharp
// Module.cs
RequiredModuleTypes.Add(typeof(Cundi.XAF.Metadata.MetadataModule));
```

Or in `Startup.cs` (Blazor/Core):

```csharp
builder.Modules.Add<Cundi.XAF.Metadata.MetadataModule>();
```

## Usage

### Automatic Synchronization
The module includes a built-in `MetadataUpdater`. Whenever the application version changes and `UpdateDatabase` is executed, the system automatically synchronizes the Metadata.

### Viewing Data
Start the application and navigate to the **Metadata Type** list view to browse all object definitions in the system.

---
[中文版](README_zh-TW.md)
