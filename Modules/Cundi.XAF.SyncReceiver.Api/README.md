# Cundi.XAF.SyncReceiver.Api Module

A DevExpress XAF Web API module that provides REST endpoints for receiving data synchronization webhooks from the `Cundi.XAF.Triggers` module.

## Features

- **Sync Endpoint**: `POST /api/Sync` for receiving single sync requests
- **Batch Sync Endpoint**: `POST /api/Sync/batch` for processing multiple sync requests
- **Health Check**: `GET /api/Sync/health` for monitoring API status
- **Authentication**: Integrates with `Cundi.XAF.ApiKey.Api` for API Key authentication
- **Type Mappings**: Support for mapping source types to local types via dependency injection

## Installation

1. Add project reference to your WebApi project:

```xml
<ProjectReference Include="path\to\Cundi.XAF.SyncReceiver.Api\Cundi.XAF.SyncReceiver.Api.csproj" />
```

2. Register the module and services in your Startup.cs:

```csharp
// Register the module
builder.Modules
    .Add<Cundi.XAF.SyncReceiver.Api.SyncReceiverApiModule>()
    .Add<Cundi.XAF.ApiKey.Api.ApiKeyApiModule>(); // For API Key authentication

// Configure type mappings (required)
services.AddSingleton<SyncTypeMappings>(sp =>
{
    var mappings = new SyncTypeMappings();
    mappings.AddMapping<YourLocalType>("Source.Namespace.SourceType");
    return mappings;
});

// Register SyncService as scoped
services.AddScoped<SyncService>();
```

## API Endpoints

### POST /api/Sync

Receives a single sync webhook request.

**Request Body:**
```json
{
  "eventType": "Created",
  "objectType": "SourceNamespace.Customer",
  "objectKey": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "triggerRule": "CustomerSync",
  "data": {
    "Name": "John Doe",
    "Email": "john@example.com"
  }
}
```

**Response (Success):**
```json
{
  "success": true,
  "message": "Successfully created object with Oid 3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response (Error):**
```json
{
  "success": false,
  "message": "Unknown object type: SourceNamespace.Customer. Add a type mapping using AddTypeMapping() method."
}
```

### POST /api/Sync/batch

Processes multiple sync requests in a single API call.

**Request Body:**
```json
[
  {
    "eventType": "Created",
    "objectType": "SourceNamespace.Customer",
    "objectKey": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "data": { "Name": "Customer 1" }
  },
  {
    "eventType": "Modified",
    "objectType": "SourceNamespace.Customer",
    "objectKey": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
    "data": { "Name": "Customer 2 Updated" }
  }
]
```

**Response:**
```json
{
  "success": true,
  "results": [
    {
      "objectKey": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "success": true,
      "message": "Successfully created object"
    },
    {
      "objectKey": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
      "success": true,
      "message": "Successfully modified object"
    }
  ]
}
```

### GET /api/Sync/health

Health check endpoint (no authentication required).

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-01T12:00:00.000Z"
}
```

## Authentication

The sync endpoints require authentication. Use one of the following methods:

### API Key Authentication (Recommended)

Include the `Cundi.XAF.ApiKey.Api` module and add the API Key header:

```
X-API-Key: cak_xxxxx
```

### JWT Authentication

Use the standard DevExpress XAF JWT authentication.

## Source-Side Configuration

Configure the `Cundi.XAF.Triggers` module on the source system:

1. Set Webhook URL: `https://your-target-server/api/Sync`
2. Add API Key in Custom Headers:
```json
{
  "X-API-Key": "cak_xxxxx"
}
```

## Project Structure

```
Cundi.XAF.SyncReceiver.Api/
├── Controllers/
│   └── SyncController.cs           # API endpoints
└── SyncReceiverApiModule.cs        # Module definition
```

## Dependencies

- DevExpress.ExpressApp.Api.Xpo.All
- Cundi.XAF.SyncReceiver

## Related Modules

- [Cundi.XAF.SyncReceiver](../Cundi.XAF.SyncReceiver/README.md) - Core sync receiver functionality
- [Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README.md) - Source-side trigger module
- [Cundi.XAF.ApiKey.Api](../Cundi.XAF.ApiKey.Api/README.md) - API Key authentication

## License

This project is licensed under the MIT License.
