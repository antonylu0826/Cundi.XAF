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
<ProjectReference Include="path\to\Cundi.XAF.Core.Api\Cundi.XAF.Core.Api.csproj" />
<ProjectReference Include="path\to\Cundi.XAF.DataMirror.Api\Cundi.XAF.DataMirror.Api.csproj" />
```

2. 在 Startup.cs 中註冊模組和服務：

```csharp
using Cundi.XAF.DataMirror.Extensions;
using Cundi.XAF.Core.Api.Extensions;

// 在 ConfigureServices 方法中
builder.Modules
    .Add<Cundi.XAF.DataMirror.Api.DataMirrorApiModule>();

// 註冊 DataMirror 服務（包含 MirroredObjectDataServicePlugin）
services.AddDataMirror();

// 註冊 CompositeDataService（必須在所有插件之後呼叫）
services.AddCompositeDataService();
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

## API 寫入保護

當呼叫 `services.AddDataMirror()` 時，標記 `[MirroredObjectProtection(true)]` 的 `MirroredObject` 衍生類型將自動受到 API 修改保護：

- ✅ **GET 請求**：允許（透過 OData 端點讀取資料）
- ❌ **POST 請求**：禁止（無法透過 OData 新增）
- ❌ **PUT/PATCH 請求**：禁止（無法透過 OData 修改）
- ❌ **DELETE 請求**：禁止（無法透過 OData 刪除）

> **注意**：預設情況下，`MirroredObject` 衍生類別是**可透過 API 編輯的**。只有標記 `[MirroredObjectProtection(true)]` 的類型才受保護。

**受保護類型的資料修改只能透過 Mirror API 端點** (`/api/Mirror`) 進行，確保與來源系統的資料一致性。

如果用戶端嘗試透過 OData 修改受保護的 MirroredObject，將收到錯誤回應：
```json
{
  "error": "Cannot modify 'SyncedTriggerDemo': This MirroredObject type is protected and read-only in the API. Use the Mirror API endpoint (/api/Mirror) to synchronize data from the source system."
}
```

## 相關模組

- [Cundi.XAF.DataMirror](../Cundi.XAF.DataMirror/README.md) - 包含業務物件和服務的核心模組
- [Cundi.XAF.ApiKey.Api](../Cundi.XAF.ApiKey.Api/README.md) - API Key 認證

## 授權

本專案採用 MIT 授權條款。
