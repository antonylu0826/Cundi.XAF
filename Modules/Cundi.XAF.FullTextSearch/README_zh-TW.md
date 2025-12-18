# Cundi.XAF.FullTextSearch

DevExpress XAF 應用程式的全域全文檢索模組。透過單一入口搜尋所有業務物件。

## 功能特性

- **全域搜尋**: 使用單一關鍵字搜尋所有持久化業務物件
- **跨平台支援**: 同時支援 Blazor 與 WinForms 平台
- **智慧定位**: 根據平台自動調整搜尋動作的位置 (Blazor: 右上角 QuickAccess，WinForms: Tools 工具列)
- **效能保護**: 可設定每個類型及總計的結果數量上限
- **點擊導航**: 點擊搜尋結果即可開啟目標物件

## 安裝方式

在您的 XAF Module 專案中加入模組參考：

```csharp
// 在 Module.cs 中
RequiredModuleTypes.Add(typeof(Cundi.XAF.FullTextSearch.FullTextSearchModule));
```

## 使用方式

1. 啟動您的 XAF 應用程式
2. 找到 **Global Search** 動作：
   - **Blazor**: 位於頁首右上角（使用者圖示附近）
   - **WinForms**: 位於 Tools 工具列
3. 輸入搜尋關鍵字並按 Enter
4. 點擊任一結果即可導航至該物件

## 設定選項

### 自訂搜尋限制

修改 `GlobalSearchService` 屬性：

```csharp
var searchService = new GlobalSearchService(objectSpace, typesInfo)
{
    MaxResultsPerType = 50,  // 每個物件類型的最大結果數
    MaxTotalResults = 200    // 總計最大結果數
};
```

### 排除特定類型

可覆寫 `GlobalSearchService` 中的 `IsSystemType` 方法來自訂排除的類型。

### 使用 Attribute 控制

使用 Attribute 控制哪些類型和屬性參與全域搜尋：

```csharp
using Cundi.XAF.FullTextSearch.Attributes;

// 將整個類型排除在全域搜尋之外
[GlobalSearchable(false)]
public class InternalConfig : BaseObject
{
    // ...
}

// 排除特定屬性
public class Customer : BaseObject
{
    public string Name { get; set; }
    
    [GlobalSearchableProperty(false)]
    public string InternalNotes { get; set; }  // 不會被搜尋
}
```

## 架構說明

| 檔案 | 說明 |
|------|------|
| `GlobalSearchResult.cs` | 非持久化的搜尋結果物件 |
| `GlobalSearchService.cs` | 核心搜尋邏輯 |
| `GlobalSearchController.cs` | Action 與 UI 處理 |
| `GlobalSearchResultNavigationController.cs` | 結果導航控制器 |

## 系統需求

- DevExpress XAF 24.2+
- .NET 8.0+
