# Cundi.XAF.SyncReceiver 模組

DevExpress XAF 資料同步接收模組，用於接收和處理來自 Webhook 的同步資料。此模組作為 `Cundi.XAF.Triggers` 模組 Webhook 功能的目的端。

## 功能特色

- **SyncableObject 基底類別**：自訂 XPO 基底類別，允許外部指定 Oid 以實現主鍵同步
- **型別映射**：使用 `SyncTypeMappings` 將來源系統型別名稱映射至本地型別
- **唯讀保護**：`SyncReadOnlyAttribute` 和 `SyncReadOnlyController` 防止在 UI 中編輯同步物件
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
using Cundi.XAF.SyncReceiver.Attributes;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;

namespace YourApp.Module.BusinessObjects;

[DefaultClassOptions]
[SyncReadOnly] // 標記為唯讀，只能透過同步 API 修改
public class SyncedCustomer : SyncableObject
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

當來源系統使用不同的型別名稱時，在 WebApi Startup 中設定映射：

```csharp
// 註冊 SyncTypeMappings 為 singleton
services.AddSingleton<SyncTypeMappings>(sp =>
{
    var mappings = new SyncTypeMappings();
    // 映射：Source.Module.Customer -> Local.Module.SyncedCustomer
    mappings.AddMapping<SyncedCustomer>("Source.Module.BusinessObjects.Customer");
    return mappings;
});

// 註冊 SyncService 為 scoped
services.AddScoped<SyncService>();
```

### SyncableObject 屬性

| 屬性 | 說明 |
|------|------|
| `Oid` | 可由外部指定的主鍵，用於同步 |
| `SyncedAt` | 從來源系統最後一次同步的時間戳記 |

### 自動 UI 保護

所有繼承 `SyncableObject` 的類別會自動：
- **隱藏 New 按鈕** - ListView 中無法新建（資料只能透過同步 API 建立）
- **隱藏 Delete 按鈕** - ListView 和 DetailView 中都無法刪除（防止意外刪除同步資料）

### SyncReadOnlyAttribute

使用此 Attribute 額外防止 UI 編輯：

```csharp
// 類別層級 - 整個物件唯讀
[SyncReadOnly]
public class SyncedCustomer : SyncableObject { }

// 允許編輯的例外情況
[SyncReadOnly(false)]
public class EditableCustomer : SyncableObject { }
```

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
├── Attributes/
│   └── SyncReadOnlyAttribute.cs    # 唯讀標記 Attribute
├── BusinessObjects/
│   └── SyncableObject.cs           # 可同步物件基底類別
├── Controllers/
│   └── SyncReadOnlyController.cs   # UI 保護控制器
├── DTOs/
│   └── SyncPayloadDto.cs           # Webhook payload 結構
├── Services/
│   ├── SyncService.cs              # 同步處理邏輯
│   └── SyncTypeMappings.cs         # 型別映射設定
└── SyncReceiverModule.cs           # 模組定義
```

## 依賴套件

- DevExpress.ExpressApp
- DevExpress.ExpressApp.Xpo
- DevExpress.Persistent.Base

## 相關模組

- [Cundi.XAF.SyncReceiver.Api](../Cundi.XAF.SyncReceiver.Api/README_zh-TW.md) - 接收 Webhook 請求的 API 端點
- [Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README_zh-TW.md) - 來源端觸發器模組，負責發送 Webhook
- [Cundi.XAF.ApiKey.Api](../Cundi.XAF.ApiKey.Api/README_zh-TW.md) - 同步端點的 API Key 認證

## 授權條款

本專案採用 MIT 授權條款。
