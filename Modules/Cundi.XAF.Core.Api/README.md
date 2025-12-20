# Cundi.XAF.Core.Api

Core WebApi infrastructure module providing the extensible DataService plugin architecture.

## Overview

This module provides `CompositeDataService` and `IDataServicePlugin` - an extensible architecture that allows multiple modules to extend DataService behavior without conflicts.

## Features

- **Plugin Architecture**: Multiple modules can register plugins that all execute when ObjectSpace is created
- **Ordered Execution**: Plugins execute in order based on their `Order` property
- **Conflict-Free**: Solves the DI container limitation where only the last registered `IDataService` takes effect

## Components

| Class | Description |
| --- | --- |
| `IDataServicePlugin` | Interface for plugins to implement |
| `CompositeDataService` | DataService that delegates to all registered plugins |
| `CoreApiExtensions` | Extension methods for service registration |

## Installation

1. Add project reference:
   ```xml
   <ProjectReference Include="path\to\Cundi.XAF.Core.Api\Cundi.XAF.Core.Api.csproj" />
   ```

2. Register in `Startup.cs`:
   ```csharp
   using Cundi.XAF.Core.Api.Extensions;
   
   // Register your module plugins first
   services.AddDataServicePlugin<YourPlugin>();
   
   // Then register CompositeDataService (MUST be last)
   services.AddCompositeDataService();
   ```

## Creating a Plugin

```csharp
public class YourPlugin : IDataServicePlugin
{
    // Lower values execute first
    public int Order => 50;
    
    public void OnObjectSpaceCreated(IObjectSpace objectSpace, Type objectType)
    {
        // Subscribe to ObjectSpace events
        objectSpace.Committing += (s, e) => { /* Your logic */ };
        objectSpace.Committed += (s, e) => { /* Your logic */ };
    }
}
```

## Built-in Plugins

| Module | Plugin | Order | Description |
| --- | --- | --- | --- |
| Cundi.XAF.DataMirror.Api | `MirroredObjectDataServicePlugin` | 0 | Blocks modifications to MirroredObject types |
| Cundi.XAF.Triggers.Api | `TriggerDataServicePlugin` | 100 | Processes trigger rules and executes webhooks |

## Requirements

- .NET 8.0+
- DevExpress XAF WebApi 24.2+

## License

MIT License
