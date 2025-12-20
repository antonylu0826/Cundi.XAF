# Samples 範例專案

此目錄包含用於測試和展示 Cundi.XAF 模組功能的範例應用程式。

## 專案結構

### Sample 系列 - 核心模組測試

| 專案 | 說明 |
|------|------|
| `Sample.Module` | 核心業務模組，包含測試用 BusinessObjects |
| `Sample.Blazor.Server` | Blazor Server 前端 |
| `Sample.WebApi` | WebAPI 後端 |
| `Sample.Win` | Windows WinForms 桌面應用 |

**測試的模組:**
- ✅ **Triggers** - 觸發器規則與 Webhook
- ✅ **FullTextSearch** - 全文搜尋功能
- ✅ **Metadata** - 型別元資料 API
- ✅ **ApiKey** - API 金鑰認證

### Receiver 系列 - DataMirror 模組測試

| 專案 | 說明 |
|------|------|
| `Receiver.Module` | 資料接收端模組 |
| `Receiver.Blazor.Server` | Blazor Server 前端 |
| `Receiver.WebApi` | WebAPI 後端 (接收同步資料) |

**測試的模組:**
- ✅ **DataMirror** - 跨系統資料鏡像同步

---

## 測試用 BusinessObjects

### Sample.Module

| 類別 | 用途 |
|------|------|
| `TriggerDemo` | 測試 Triggers 模組 - 觸發器規則 |
| `SearchableProduct` | 測試 FullTextSearch 模組 - 產品搜尋 |
| `SearchableDocument` | 測試 FullTextSearch 模組 - 文件搜尋 |

### Receiver.Module

| 類別 | 用途 |
|------|------|
| `SyncedTriggerDemo` | 測試 DataMirror 模組 - 從 Sample 同步的資料 |

---

## 快速開始

### 測試 Sample 應用

1. 設定 `Sample.Blazor.Server` 或 `Sample.Win` 為啟動專案
2. 確認連接字串配置正確
3. 啟動應用程式

### 測試 Receiver 應用

1. 先啟動 `Sample.WebApi`
2. 設定 `Receiver.Blazor.Server` 或 `Receiver.WebApi` 為啟動專案
3. 在 Sample 中建立觸發規則指向 Receiver 端點
4. 在 Sample 中操作資料，觀察 Receiver 端同步結果

---

## 連接字串配置

編輯 `appsettings.json` 檔案：

```json
{
  "ConnectionStrings": {
    "ConnectionString": "Server=.;Database=YourDatabase;Trusted_Connection=True;"
  }
}
```
