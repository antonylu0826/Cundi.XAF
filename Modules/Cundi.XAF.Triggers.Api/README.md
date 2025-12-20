# Cundi.XAF.Triggers.Api

WebApi integration module for [Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README.md).

## Overview

This module provides `TriggerDataServicePlugin` - a DataService plugin that enables trigger processing in ASP.NET Core WebApi applications. It uses the extensible DataService architecture from [Cundi.XAF.Core.Api](../Cundi.XAF.Core.Api/README.md).

## Features

- **Plugin Architecture**: Integrates with `CompositeDataService` for extensibility
- **Sync Execution**: Webhook calls are executed synchronously within the request lifecycle  
- **Full Response Logging**: Complete HTTP response details are captured in TriggerLog
- **Scoped State Management**: Thread-safe trigger state handling for concurrent requests

## Installation

1. Add project references:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\Modules\Cundi.XAF.Core.Api\Cundi.XAF.Core.Api.csproj" />
     <ProjectReference Include="..\Modules\Cundi.XAF.Triggers\Cundi.XAF.Triggers.csproj" />
     <ProjectReference Include="..\Modules\Cundi.XAF.Triggers.Api\Cundi.XAF.Triggers.Api.csproj" />
   </ItemGroup>
   ```

2. Register in `Startup.cs`:
   ```csharp
   using Cundi.XAF.Triggers.Extensions;
   using Cundi.XAF.Core.Api.Extensions;
   
   // Register Triggers plugin
   services.AddTriggers();
   
   // Register CompositeDataService (MUST be called after all plugins)
   services.AddCompositeDataService();
   ```

## How It Works

```
API Request → CompositeDataService.CreateObjectSpace()
                           ↓
              TriggerDataServicePlugin.OnObjectSpaceCreated()
                           ↓
              ObjectSpace.Committing Event
              - Validate objects
              - Capture object states
                           ↓
              ObjectSpace.Committed Event
              - Execute webhooks (sync)
              - Log with full response
                           ↓
              API Response ← Return to client
```

## Requirements

- .NET 8.0+
- DevExpress XAF WebApi 24.2+
- Cundi.XAF.Core.Api module
- Cundi.XAF.Triggers module

## License

MIT License
