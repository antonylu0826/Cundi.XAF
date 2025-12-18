# Cundi.XAF.Metadata.Api

**Cundi.XAF.Metadata.Api** is an extension module for `Cundi.XAF.Metadata` that provides RESTful API endpoints, enabling external systems to access object metadata within the XAF system.

## Features

- **RESTful API**: Provides standard HTTP GET endpoints.
- **Lightweight DTOs**: Uses Data Transfer Objects (DTOs) for data transmission, avoiding circular references and excessive payloads.
- **Detailed Information Query**: Supports querying complete property information for a single type, including field names, captions, and types.

## API Endpoints

### Get All Types
```http
GET /api/Metadata/Types
```
**Response Example**:
```json
[
  {
    "typeName": "ApplicationUser",
    "fullName": "Cundi.XAF.Module.BusinessObjects.ApplicationUser",
    "assemblyName": "Cundi.XAF.Module, Version=1.0..."
  },
  ...
]
```

### Get Specific Type Details
```http
GET /api/Metadata/Type/{fullName}
```
**Parameters**:
- `fullName`: The full type name of the object (e.g., `Cundi.XAF.Module.BusinessObjects.ApplicationUser`)

**Response Example**:
```json
{
  "typeName": "ApplicationUser",
  "fullName": "Cundi.XAF.Module.BusinessObjects.ApplicationUser",
  "properties": [
    {
      "propertyName": "UserName",
      "propertyType": "System.String",
      "friendlyPropertyType": "String",
      "caption": "User Name"
    },
    ...
  ]
}
```

## Installation

Add this module to your WebApi project's `Module.cs` or `Startup.cs`:

```csharp
// Module.cs
RequiredModuleTypes.Add(typeof(Cundi.XAF.Metadata.Api.MetadataApiModule));
```

> **Note**: This module depends on the `Cundi.XAF.Metadata` module.

---
[中文版](README_zh-TW.md)
