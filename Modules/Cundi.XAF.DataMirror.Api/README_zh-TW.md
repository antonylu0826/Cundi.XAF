# Cundi.XAF.DataMirror.Api 模組

用於接收來自 `Cundi.XAF.Triggers` 模組 Webhook 請求的 API 模組。提供 REST API 端點處理傳入的鏡像資料並將變更套用至本地資料庫。

## 功能特色

- **REST API 端點**：`/api/Mirror` 用於接收單一 Webhook 請求
- **批次處理**：`/api/Mirror/batch` 用於一次處理多個請求
- **健康檢查**：`/api/Mirror/health` 端點用於監控
- **認證**：支援 JWT 和 API Key 認證

## 安裝方式

1. 在 WebApi 專案中加入專案參考：

```xml
<ProjectReference Include="path\to\Cundi.XAF.DataMirror.Api\Cundi.XAF.DataMirror.Api.csproj" />
```

2. 在 Startup.cs 中註冊模組和服務：

```csharp
using Cundi.XAF.DataMirror.Extensions;

// 在 ConfigureServices 方法中
builder.Modules
    .Add<Cundi.XAF.DataMirror.Api.DataMirrorApiModule>();

// 註冊 DataMirror 服務
services.AddDataMirror();
```

## API 端點

### POST /api/Mirror

接收單一鏡像 Webhook 請求。

**請求內容：**
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

**回應 (200 OK)：**
```json
{
  "success": true,
  "message": "Successfully created object with Oid xxx"
}
```

### POST /api/Mirror/batch

一次處理多個鏡像請求。

**請求內容：**
```json
[
  { "eventType": "Created", "objectType": "...", ... },
  { "eventType": "Modified", "objectType": "...", ... }
]
```

**回應：**
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

健康檢查端點（不需要認證）。

**回應：**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-01T00:00:00.000Z"
}
```

## 認證方式

此 API 需要認證。支援：
- **JWT Bearer Token**：使用 `/api/Authentication/Authenticate` 取得令牌
- **API Key**：使用 `X-API-Key` 標頭並提供有效的 API Key

## 相關模組

- [Cundi.XAF.DataMirror](../Cundi.XAF.DataMirror/README.md) - 包含業務物件和服務的核心模組
- [Cundi.XAF.ApiKey.Api](../Cundi.XAF.ApiKey.Api/README.md) - API Key 認證

## 授權

本專案採用 MIT 授權條款。
