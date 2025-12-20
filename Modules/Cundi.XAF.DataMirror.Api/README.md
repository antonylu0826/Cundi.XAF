# Cundi.XAF.DataMirror.Api Module

API module for receiving webhook requests from the `Cundi.XAF.Triggers` module. Provides REST API endpoints to process incoming mirror data and apply changes to the local database.

## Features

- **REST API Endpoint**: `/api/Mirror` for receiving single webhook requests
- **Batch Processing**: `/api/Mirror/batch` for processing multiple requests at once
- **Health Check**: `/api/Mirror/health` endpoint for monitoring
- **Authentication**: Supports JWT and API Key authentication

## Installation

1. Add project reference to your WebApi project:

```xml
<ProjectReference Include="path\to\Cundi.XAF.DataMirror.Api\Cundi.XAF.DataMirror.Api.csproj" />
```

2. Register the module and services in Startup.cs:

```csharp
using Cundi.XAF.DataMirror.Extensions;

// In ConfigureServices method
builder.Modules
    .Add<Cundi.XAF.DataMirror.Api.DataMirrorApiModule>();

// Register DataMirror services
services.AddDataMirror();
```

## API Endpoints

### POST /api/Mirror

Receives a single mirror webhook request.

**Request Body:**
```json
{
  "eventType": "Created",
  "objectType": "Sample.Module.BusinessObjects.TriggerDemo",
  "objectKey": "guid-string",
  "timestamp": "2024-01-01T00:00:00.000Z",
  "triggerRule": "RuleName",
  "data": {
    "Name": "Demo Object"
  }
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Successfully created object with Oid xxx"
}
```

### POST /api/Mirror/batch

Processes multiple mirror requests at once.

**Request Body:**
```json
[
  { "eventType": "Created", "objectType": "...", ... },
  { "eventType": "Modified", "objectType": "...", ... }
]
```

**Response:**
```json
{
  "success": true,
  "results": [
    { "objectKey": "...", "success": true, "message": "..." },
    { "objectKey": "...", "success": true, "message": "..." }
  ]
}
```

### GET /api/Mirror/health

Health check endpoint (no authentication required).

**Response:**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-01T00:00:00.000Z"
}
```

## Authentication

This API requires authentication. Supports:
- **JWT Bearer Token**: Use `/api/Authentication/Authenticate` to obtain a token
- **API Key**: Use `X-API-Key` header with a valid API key

## API Write Protection

When `services.AddDataMirror()` is called, all `MirroredObject`-derived types are automatically protected from API modifications:

- ✅ **GET requests**: Allowed (read data via OData endpoints)
- ❌ **POST requests**: Blocked (cannot create via OData)
- ❌ **PUT/PATCH requests**: Blocked (cannot update via OData)
- ❌ **DELETE requests**: Blocked (cannot delete via OData)

**Data modification is only allowed via the Mirror API endpoint** (`/api/Mirror`), ensuring data integrity with the source system.

If a client attempts to modify a MirroredObject via OData, they will receive an error response:
```json
{
  "error": "Cannot modify 'SyncedTriggerDemo': MirroredObject types are read-only in the API. Use the Mirror API endpoint (/api/Mirror) to synchronize data from the source system."
}
```

## Related Modules

- [Cundi.XAF.DataMirror](../Cundi.XAF.DataMirror/README.md) - Core module with business objects and services
- [Cundi.XAF.ApiKey.Api](../Cundi.XAF.ApiKey.Api/README.md) - API Key authentication

## License

This project is licensed under the MIT License.

