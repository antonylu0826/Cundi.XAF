# Cundi.XAF.Metadata

**Cundi.XAF.Metadata** 是一個 XAF 模組，專門用於掃描並記錄系統中的 Persistent Object 資訊。
它會自動將系統中的 Business Object 類型及其屬性同步到資料庫中，供其他模組（如報表、權限控制、API）使用。

## 功能特性

- **自動掃描**：在資料庫架構更新時，自動掃描所有標記為 `Persistent` 且 `Visible` 的類型。
- **資料持久化**：
  - `MetadataType`: 記錄物件類型的完整名稱 (Full Name) 與組件名稱 (Assembly Name)。
  - `MetadataProperty`: 記錄物件屬性，包含原始型別 (Full Name) 與易讀型別名稱 (Friendly Name, 如 `List<String>`)。
- **系統維護保護**：所有的 Metadata 資料在 UI 上皆為**唯讀 (Read-Only)** 且**不可手動建立**，確保資料的一致性與準確性。
- **手動更新機制**：提供 Action 供管理員在執行時手動觸發 Metadata 更新。

## 安裝

在您的 XAF 專案（Blazor, WinForms, 或 WebApi）的 `Module.cs` 或 `Startup.cs` 中加入此模組：

```csharp
// Module.cs
RequiredModuleTypes.Add(typeof(Cundi.XAF.Metadata.MetadataModule));
```

或在 `Startup.cs` (Blazor/Core) 中：

```csharp
builder.Modules.Add<Cundi.XAF.Metadata.MetadataModule>();
```

## 使用方法

### 自動同步
模組已內建 `MetadataUpdater`。每當應用程式版本更新且執行 `UpdateDatabase` 時，系統會自動同步 Metadata。

### 查看資料
啟動應用程式後，導航至 **Metadata Type** 列表視圖，即可查看系統中所有的物件定義。

---
[English Version](README.md)
