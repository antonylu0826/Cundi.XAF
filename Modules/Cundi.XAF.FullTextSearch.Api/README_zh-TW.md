# Cundi.XAF.FullTextSearch.Api

[FullTextSearch](../Cundi.XAF.FullTextSearch/README_zh-TW.md) 模組的 Web API 擴充。提供 RESTful API 端點，讓外部系統可進行全域全文搜尋。

## 功能特色

- **RESTful API**：標準 Web API 端點進行全域搜尋
- **可設定限制**：控制每次請求的最大結果數
- **預設安全**：需要透過 `[Authorize]` 進行身份驗證
- **豐富回應**：回傳顯示名稱、物件 Key、類型資訊及匹配內容

## 安裝

1. 在您的 WebApi 專案中新增專案參照：

```xml
<ProjectReference Include="..\Modules\Cundi.XAF.FullTextSearch.Api\Cundi.XAF.FullTextSearch.Api.csproj" />
```

2. 在應用程式中註冊模組：

```csharp
RequiredModuleTypes.Add(typeof(Cundi.XAF.FullTextSearch.Api.FullTextSearchApiModule));
```

## API 參考

### 搜尋端點

```
GET /api/GlobalSearch?keyword={keyword}&maxResults={maxResults}&maxPerType={maxPerType}
```

#### 參數

| 參數 | 類型 | 必填 | 預設值 | 說明 |
|------|------|------|--------|------|
| `keyword` | string | 是 | - | 搜尋關鍵字 |
| `maxResults` | int | 否 | 200 | 最大結果總數 |
| `maxPerType` | int | 否 | 50 | 每種類型的最大結果數 |

#### 回應範例

```json
{
  "keyword": "john",
  "totalCount": 3,
  "results": [
    {
      "displayName": "John Doe",
      "objectKey": "abc123-def456",
      "typeCaption": "Customer",
      "typeFullName": "Sample.Module.BusinessObjects.Customer",
      "matchedContent": "Name: John Doe | Email: john@example.com"
    }
  ]
}
```

#### 回應代碼

| 代碼 | 說明 |
|------|------|
| 200 | 成功 |
| 400 | 請求錯誤（缺少關鍵字） |
| 401 | 未授權 |

## 使用範例

### cURL

```bash
curl -X GET "https://localhost:5001/api/GlobalSearch?keyword=test" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### JavaScript (Fetch)

```javascript
const response = await fetch('/api/GlobalSearch?keyword=test', {
  headers: { 'Authorization': 'Bearer ' + token }
});
const data = await response.json();
console.log(data.results);
```

## 架構

| 檔案 | 說明 |
|------|------|
| `FullTextSearchApiModule.cs` | XAF 模組定義 |
| `GlobalSearchController.cs` | API 控制器 |
| `GlobalSearchDTOs.cs` | 資料傳輸物件 |

## 需求

- DevExpress XAF 24.2+
- .NET 8.0+
- [Cundi.XAF.FullTextSearch](../Cundi.XAF.FullTextSearch/README_zh-TW.md) 模組
