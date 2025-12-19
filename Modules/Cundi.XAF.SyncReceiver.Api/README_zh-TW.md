# Cundi.XAF.SyncReceiver.Api 模組

DevExpress XAF Web API 模組，提供 REST 端點來接收來自 `Cundi.XAF.Triggers` 模組的資料同步 Webhook。

## 功能特色

- **同步端點**：`POST /api/Sync` 接收單一同步請求
- **批次同步端點**：`POST /api/Sync/batch` 處理多個同步請求
- **健康檢查**：`GET /api/Sync/health` 監控 API 狀態
- **認證整合**：與 `Cundi.XAF.ApiKey.Api` 整合進行 API Key 認證
- **型別映射**：支援透過依賴注入將來源型別映射至本地型別

## 安裝

1. 在您的 WebApi 專案中加入專案參考：

```xml
<ProjectReference Include="path\to\Cundi.XAF.SyncReceiver.Api\Cundi.XAF.SyncReceiver.Api.csproj" />
```

2. 在 Startup.cs 中註冊模組和服務：

```csharp
// 註冊模組
builder.Modules
    .Add<Cundi.XAF.SyncReceiver.Api.SyncReceiverApiModule>()
    .Add<Cundi.XAF.ApiKey.Api.ApiKeyApiModule>(); // API Key 認證

// 設定型別映射（必要）
services.AddSingleton<SyncTypeMappings>(sp =>
{
    var mappings = new SyncTypeMappings();
    mappings.AddMapping<YourLocalType>("Source.Namespace.SourceType");
    return mappings;
});

// 註冊 SyncService 為 scoped
services.AddScoped<SyncService>();
```

## API 端點

### POST /api/Sync

接收單一同步 Webhook 請求。

**請求主體：**
```json
{
  "eventType": "Created",
  "objectType": "SourceNamespace.Customer",
  "objectKey": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "timestamp": "2024-01-01T12:00:00.000Z",
  "triggerRule": "CustomerSync",
  "data": {
    "Name": "張三",
    "Email": "john@example.com"
  }
}
```

**回應（成功）：**
```json
{
  "success": true,
  "message": "Successfully created object with Oid 3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**回應（錯誤）：**
```json
{
  "success": false,
  "message": "Unknown object type: SourceNamespace.Customer. Add a type mapping using AddTypeMapping() method."
}
```

### POST /api/Sync/batch

在單一 API 呼叫中處理多個同步請求。

**請求主體：**
```json
[
  {
    "eventType": "Created",
    "objectType": "SourceNamespace.Customer",
    "objectKey": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "data": { "Name": "客戶 1" }
  },
  {
    "eventType": "Modified",
    "objectType": "SourceNamespace.Customer",
    "objectKey": "4fa85f64-5717-4562-b3fc-2c963f66afa7",
    "data": { "Name": "客戶 2 已更新" }
  }
]
```

**回應：**
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

健康檢查端點（不需要認證）。

**回應：**
```json
{
  "status": "healthy",
  "timestamp": "2024-01-01T12:00:00.000Z"
}
```

## 認證

同步端點需要認證。使用以下方法之一：

### API Key 認證（建議）

包含 `Cundi.XAF.ApiKey.Api` 模組並加入 API Key 標頭：

```
X-API-Key: cak_xxxxx
```

### JWT 認證

使用標準的 DevExpress XAF JWT 認證。

## 來源端設定

在來源系統中設定 `Cundi.XAF.Triggers` 模組：

1. 設定 Webhook URL：`https://your-target-server/api/Sync`
2. 在自訂標頭中加入 API Key：
```json
{
  "X-API-Key": "cak_xxxxx"
}
```

## 專案結構

```
Cundi.XAF.SyncReceiver.Api/
├── Controllers/
│   └── SyncController.cs           # API 端點
└── SyncReceiverApiModule.cs        # 模組定義
```

## 依賴套件

- DevExpress.ExpressApp.Api.Xpo.All
- Cundi.XAF.SyncReceiver

## 相關模組

- [Cundi.XAF.SyncReceiver](../Cundi.XAF.SyncReceiver/README_zh-TW.md) - 同步接收器核心功能
- [Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README_zh-TW.md) - 來源端觸發器模組
- [Cundi.XAF.ApiKey.Api](../Cundi.XAF.ApiKey.Api/README_zh-TW.md) - API Key 認證

## 授權條款

本專案採用 MIT 授權條款。
