# Cundi.XAF.DataMirror 模組

DevExpress XAF 模組，用於接收和處理來自 Webhook 的鏡像資料。此模組作為 `Cundi.XAF.Triggers` 模組 Webhook 功能的目標端點。

## 功能特色

- **MirroredObject 基類**：自訂 XPO 基類，允許外部設定 Oid 以進行主鍵同步
- **動態類型對應**：透過 XAF UI (MirrorTypeMappingConfig) 設定來源到本地類型的對應，無需修改程式碼
- **自動唯讀保護**：所有繼承自 `MirroredObject` 的類別在 UI 中自動設為唯讀
- **自動隱藏新增/刪除**：自動隱藏 MirroredObject 衍生類別的新增和刪除按鈕
- **鏡像服務**：處理傳入的 Webhook 資料並套用建立/修改/刪除操作
- **Upsert 支援**：在修改事件中若物件不存在則自動建立
- **限制管理員存取**：類型對應設定只有管理員才能存取

## 安裝方式

1. 在 XAF 模組專案中加入專案參考：

```xml
<ProjectReference Include="path\to\Cundi.XAF.DataMirror\Cundi.XAF.DataMirror.csproj" />
```

2. 在應用程式中註冊模組：

```csharp
RequiredModuleTypes.Add(typeof(Cundi.XAF.DataMirror.DataMirrorModule));
```

## 使用方式

### 建立鏡像業務物件

建立繼承自 `MirroredObject` 的業務物件：

```csharp
using Cundi.XAF.DataMirror.BusinessObjects;
using DevExpress.Xpo;
using DevExpress.Persistent.Base;

namespace YourApp.Module.BusinessObjects;

[DefaultClassOptions]
public class SyncedCustomer : MirroredObject  // 在 UI 中自動唯讀
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

### 類型對應

類型對應透過 XAF UI 使用 `MirrorTypeMappingConfig` 動態設定：

1. 在 XAF 應用程式中導航至 **設定 > 鏡像類型對應**
2. 建立新的對應：
   - **來源類型名稱**：來源系統的完整類型名稱（例如 `Sample.Module.BusinessObjects.TriggerDemo`）
   - **本地類型**：從下拉選單中選擇所有 `MirroredObject` 子類別
   - **啟用**：啟用此對應

> **注意**：類型下拉選單只會顯示繼承自 `MirroredObject` 的類別。
> **注意**：類型對應設定只有管理員使用者才能存取。

### MirroredObject 屬性

| 屬性 | 說明 |
|------|------|
| `Oid` | 可從外部設定以進行同步的主鍵 |
| `SyncedAt` | 從來源系統最後一次同步的時間戳記 |

### 自動 UI 保護

所有繼承自 `MirroredObject` 的類別自動：
- **DetailView 唯讀**（資料只能透過 Mirror API 修改）
- **隱藏 ListView 新增按鈕**（資料只能透過 Mirror API 建立）
- **在 ListView 和 DetailView 中隱藏刪除按鈕**（防止意外刪除）

## Webhook 資料格式

模組預期的資料格式與 `Cundi.XAF.Triggers` 傳送的格式相同：

```json
{
  "eventType": "Created | Modified | Deleted",
  "objectType": "SourceNamespace.Customer",
  "objectKey": "guid-string",
  "timestamp": "2024-01-01T00:00:00.000Z",
  "triggerRule": "RuleName",
  "data": {
    "Name": "Customer Name",
    "Email": "customer@example.com"
  }
}
```

### 事件類型

| 事件 | 行為 |
|------|------|
| `Created` | 使用指定的 Oid 建立新物件 |
| `Modified` | 更新現有物件，若不存在則建立 (Upsert) |
| `Deleted` | 刪除物件（若已刪除或不存在則不報錯） |

## 專案結構

```
Cundi.XAF.DataMirror/
├── BusinessObjects/
│   ├── MirroredObject.cs              # 鏡像物件基類
│   └── MirrorTypeMappingConfig.cs     # 動態類型對應設定
├── Controllers/
│   ├── MirrorReadOnlyController.cs    # UI 保護控制器
│   └── MirrorTypeMappingAdminController.cs  # 管理員權限控制
├── DTOs/
│   └── MirrorPayloadDto.cs            # Webhook 資料結構
├── Services/
│   ├── MirrorService.cs               # 鏡像處理邏輯
│   └── MirrorTypeMappings.cs          # 類型對應服務
├── TypeConverters/
│   └── MirroredTypeConverter.cs       # MirroredObject 子類別 UI 選擇器
└── DataMirrorModule.cs                # 模組定義
```

## 相依性

- DevExpress.ExpressApp
- DevExpress.ExpressApp.Xpo
- DevExpress.ExpressApp.Security
- DevExpress.Persistent.Base
- DevExpress.Persistent.BaseImpl.Xpo

## 相關模組

- [Cundi.XAF.DataMirror.Api](../Cundi.XAF.DataMirror.Api/README.md) - 接收 Webhook 請求的 API 端點
- [Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README.md) - 發送 Webhook 的來源端觸發器模組
- [Cundi.XAF.ApiKey.Api](../Cundi.XAF.ApiKey.Api/README.md) - 鏡像端點的 API Key 認證

## 授權

本專案採用 MIT 授權條款。
