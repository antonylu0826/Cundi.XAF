# Cundi.XAF.SyncReceiver 模組

DevExpress XAF 資料同步接收模組，用於接收和處理來自 Webhook 的同步資料。此模組作為 `Cundi.XAF.Triggers` 模組 Webhook 功能的目的端。

## 功能特色

- **SyncableObject 基底類別**：自訂 XPO 基底類別，允許外部指定 Oid 以實現主鍵同步
- **動態型別映射**：透過 XAF UI 設定來源到本地型別的對應關係（SyncTypeMappingConfig），無需修改程式碼
- **自動唯讀保護**：所有繼承 `SyncableObject` 的類別自動在 UI 中唯讀
- **自動隱藏 New/Delete**：自動隱藏所有繼承 SyncableObject 類別的新增和刪除按鈕
- **同步服務**：處理傳入的 Webhook payload 並執行新增/修改/刪除操作
- **Upsert 支援**：當 Modified 事件的物件不存在時，自動建立物件

## 安裝

1. 在您的 XAF 模組專案中加入專案參考：

```xml
<ProjectReference Include="path\to\Cundi.XAF.SyncReceiver\Cundi.XAF.SyncReceiver.csproj" />
```

2. 在您的應用程式中註冊模組：

```csharp
RequiredModuleTypes.Add(typeof(Cundi.XAF.SyncReceiver.SyncReceiverModule));
```

## 使用方式

### 建立可同步的 Business Object

建立繼承自 `SyncableObject` 的 Business Object：

```csharp
using Cundi.XAF.SyncReceiver.BusinessObjects;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;

namespace YourApp.Module.BusinessObjects;

[DefaultClassOptions]
public class SyncedCustomer : SyncableObject  // 在 UI 中自動唯讀
{
    public SyncedCustomer(Session session) : base(session) { }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set => SetPropertyValue(nameof(Name), ref _name, value);
    }
}
```

### 型別映射

型別映射現在透過 XAF UI 使用 `SyncTypeMappingConfig` 動態設定：

1. 在您的 XAF 應用程式中導航至 **Configuration > Sync Type Mapping Config**
2. 建立新的映射：
   - **Source Type Name**：來源系統的完整型別名稱（例如：`Sample.Module.BusinessObjects.TriggerDemo`）
   - **Local Type**：從下拉選單選擇所有 `SyncableObject` 子類別
   - **Is Active**：啟用對應

在您的 WebApi Startup.cs 中，只需註冊服務：

```csharp
using Cundi.XAF.SyncReceiver.Extensions;

// 一行註冊所有 SyncReceiver 服務
services.AddSyncReceiver();
```

> **說明**：型別下拉選單只會顯示繼承自 `SyncableObject` 的類別。

### SyncableObject 屬性

| 屬性 | 說明 |
|------|------|
| `Oid` | 可由外部指定的主鍵，用於同步 |
| `SyncedAt` | 從來源系統最後一次同步的時間戳記 |

### 自動 UI 保護

所有繼承 `SyncableObject` 的類別會自動：
- **DetailView 唯讀** - 資料只能透過同步 API 修改
- **隱藏 New 按鈕** - ListView 中無法新建（資料只能透過同步 API 建立）
- **隱藏 Delete 按鈕** - ListView 和 DetailView 中都無法刪除（防止意外刪除同步資料）

## Webhook Payload 格式

模組預期接收的 payload 格式與 `Cundi.XAF.Triggers` 傳送的格式相同：

```json
{
  "eventType": "Created | Modified | Deleted",
  "objectType": "SourceNamespace.Customer",
  "objectKey": "guid-string",
  "timestamp": "2024-01-01T00:00:00.000Z",
  "triggerRule": "RuleName",
  "data": {
    "Name": "客戶名稱",
    "Email": "customer@example.com"
  }
}
```

### 事件類型

| 事件 | 行為 |
|------|------|
| `Created` | 以指定的 Oid 建立新物件 |
| `Modified` | 更新現有物件，若不存在則自動建立（Upsert） |
| `Deleted` | 刪除物件（若已刪除則不報錯） |

## 專案結構

```
Cundi.XAF.SyncReceiver/
├── BusinessObjects/
│   ├── SyncableObject.cs           # 可同步物件基底類別
│   └── SyncTypeMappingConfig.cs    # 動態型別映射設定
├── Controllers/
│   └── SyncReadOnlyController.cs   # UI 保護控制器
├── DTOs/
│   └── SyncPayloadDto.cs           # Webhook payload 結構
├── Services/
│   ├── SyncService.cs              # 同步處理邏輯
│   └── SyncTypeMappings.cs         # 型別映射服務
├── TypeConverters/
│   └── SyncableTypeConverter.cs    # UI 型別選擇器
└── SyncReceiverModule.cs           # 模組定義
```

## 依賴套件

- DevExpress.ExpressApp
- DevExpress.ExpressApp.Xpo
- DevExpress.Persistent.Base
- DevExpress.Persistent.BaseImpl.Xpo

## 相關模組

- [Cundi.XAF.SyncReceiver.Api](../Cundi.XAF.SyncReceiver.Api/README_zh-TW.md) - 接收 Webhook 請求的 API 端點
- [Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README_zh-TW.md) - 來源端觸發器模組，負責發送 Webhook
- [Cundi.XAF.ApiKey.Api](../Cundi.XAF.ApiKey.Api/README_zh-TW.md) - 同步端點的 API Key 認證

## 授權條款

本專案採用 MIT 授權條款。
