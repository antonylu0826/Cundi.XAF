# Cundi.XAF.Triggers.Api

WebApi integration module for [Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README.md).

## Overview

This module provides `WebApiMiddleDataService` - a custom `IDataService` implementation that enables trigger processing in ASP.NET Core WebApi applications.

## Features

- **Sync Execution**: Webhook calls are executed synchronously within the request lifecycle
- **Full Response Logging**: Complete HTTP response details are captured in TriggerLog
- **Scoped State Management**: Thread-safe trigger state handling for concurrent requests

## Installation

1. Add project references:
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\Modules\Cundi.XAF.Triggers\Cundi.XAF.Triggers.csproj" />
     <ProjectReference Include="..\Modules\Cundi.XAF.Triggers.Api\Cundi.XAF.Triggers.Api.csproj" />
   </ItemGroup>
   ```

2. Register in `Startup.cs`:
   ```csharp
   using Cundi.XAF.Triggers.Api;
   
   services.AddScoped<IDataService, WebApiMiddleDataService>();
   ```

## How It Works

```
API Request → WebApiMiddleDataService.CreateObjectSpace()
                           ↓
              ObjectSpace.Committing Event
              - Capture object states
              - Store in instance field (scoped)
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
- Cundi.XAF.Triggers module

## License

MIT License
