# Cundi.XAF.FullTextSearch.Api

Web API extension for the [FullTextSearch](../Cundi.XAF.FullTextSearch/README.md) module. Provides RESTful API endpoints for global full-text search across all business objects.

## Features

- **RESTful API**: Standard Web API endpoint for global search
- **Configurable Limits**: Control maximum results per request
- **Secure by Default**: Requires authentication via `[Authorize]`
- **Rich Response**: Returns display name, object key, type info, and matched content

## Installation

1. Add project reference to your WebApi project:

```xml
<ProjectReference Include="..\Modules\Cundi.XAF.FullTextSearch.Api\Cundi.XAF.FullTextSearch.Api.csproj" />
```

2. Register the module in your application:

```csharp
RequiredModuleTypes.Add(typeof(Cundi.XAF.FullTextSearch.Api.FullTextSearchApiModule));
```

## API Reference

### Search Endpoint

```
GET /api/GlobalSearch?keyword={keyword}&maxResults={maxResults}&maxPerType={maxPerType}
```

#### Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `keyword` | string | Yes | - | Search keyword |
| `maxResults` | int | No | 200 | Maximum total results |
| `maxPerType` | int | No | 50 | Maximum results per type |

#### Response Example

```json
{
  "keyword": "john",
  "totalCount": 3,
  "results": [
    {
      "displayName": "John Doe",
      "objectKey": "abc123-def456",
      "typeCaption": "Customer",
      "typeFullName": "Sample.Module.BusinessObjects.Customer",
      "matchedContent": "Name: John Doe | Email: john@example.com"
    }
  ]
}
```

#### Response Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 400 | Bad Request (missing keyword) |
| 401 | Unauthorized |

## Usage Examples

### cURL

```bash
curl -X GET "https://localhost:5001/api/GlobalSearch?keyword=test" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### JavaScript (Fetch)

```javascript
const response = await fetch('/api/GlobalSearch?keyword=test', {
  headers: { 'Authorization': 'Bearer ' + token }
});
const data = await response.json();
console.log(data.results);
```

## Architecture

| File | Description |
|------|-------------|
| `FullTextSearchApiModule.cs` | XAF module definition |
| `GlobalSearchController.cs` | API controller |
| `GlobalSearchDTOs.cs` | Data transfer objects |

## Requirements

- DevExpress XAF 24.2+
- .NET 8.0+
- [Cundi.XAF.FullTextSearch](../Cundi.XAF.FullTextSearch/README.md) module
