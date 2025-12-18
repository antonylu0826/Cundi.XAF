# Cundi.XAF.Metadata.Api

**Cundi.XAF.Metadata.Api** 是 `Cundi.XAF.Metadata` 的擴充模組，提供 RESTful API 接口，讓外部系統能夠存取 XAF 系統中的物件元數據。

## 功能特性

- **RESTful API**: 提供標準的 HTTP GET 端點。
- **輕量級 DTO**: 使用 Data Transfer Objects (DTOs) 傳輸資料，避免循環引用與過大的 Payload。
- **詳細資訊查詢**: 支援查詢單一類型的完整屬性資訊，包含欄位名稱、顯示名稱 (Caption) 與型別。

## API 端點

### 取得所有類型列表
```http
GET /api/Metadata/Types
```
**回傳範例**:
```json
[
  {
    "typeName": "ApplicationUser",
    "fullName": "Cundi.XAF.Module.BusinessObjects.ApplicationUser",
    "assemblyName": "Cundi.XAF.Module, Version=1.0..."
  },
  ...
]
```

### 取得特定類型詳細資訊
```http
GET /api/Metadata/Type/{fullName}
```
**參數**:
- `fullName`: 物件的完整類型名稱 (例如 `Cundi.XAF.Module.BusinessObjects.ApplicationUser`)

**回傳範例**:
```json
{
  "typeName": "ApplicationUser",
  "fullName": "Cundi.XAF.Module.BusinessObjects.ApplicationUser",
  "properties": [
    {
      "propertyName": "UserName",
      "propertyType": "System.String",
      "friendlyPropertyType": "String",
      "caption": "User Name"
    },
    ...
  ]
}
```

## 安裝

在您的 WebApi 專案的 `Module.cs` 或 `Startup.cs` 中加入此模組：

```csharp
// Module.cs
RequiredModuleTypes.Add(typeof(Cundi.XAF.Metadata.Api.MetadataApiModule));
```

> **注意**: 此模組依賴於 `Cundi.XAF.Metadata` 模組。

---
[English Version](README.md)
