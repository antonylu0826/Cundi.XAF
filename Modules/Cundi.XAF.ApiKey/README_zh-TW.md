# Cundi.XAF.ApiKey

用於產生、管理和驗證 API Key 的獨立 XAF 模組。

## 功能特色

- **安全金鑰生成**：使用 `RandomNumberGenerator`（256 位元）搭配 SHA256 雜湊
- **金鑰過期設定**：可配置過期時間（5 分鐘至 90 天）
- **僅限管理員**：只有 `IsAdministrative` 角色的用戶才能管理金鑰
- **每用戶一組金鑰**：產生新金鑰會自動撤銷舊金鑰
- **使用追蹤**：記錄建立時間和最後使用時間

## 安裝方式

在您的 XAF 模組專案中加入 `Cundi.XAF.ApiKey` 參考。

```csharp
// 在模組建構函式中
RequiredModuleTypes.Add(typeof(Cundi.XAF.ApiKey.ApiKeyModule));
```

## 使用方式

### 產生 API Key

1. 開啟任意用戶的 **DetailView**（需要 `IsAdministrative` 角色）
2. 點擊工具列中的 **「Generate API Key」**
3. 選擇過期時間：5 分鐘 / 30 分鐘 / 1 天 / 30 天 / 60 天 / 90 天
4. 彈出視窗顯示產生的金鑰（已自動複製到剪貼簿）

> ⚠️ **重要**：金鑰僅顯示一次，之後無法再取得。

### 撤銷 API Key

1. 開啟用戶的 DetailView
2. 點擊工具列中的 **「Revoke API Key」**
3. 確認操作

### 管理 API Key

API Key 儲存在 `ApiKeyInfo` 實體中，可透過 **Security** 導航存取。

| 屬性 | 說明 |
|------|------|
| `UserOid` | 關聯用戶的 Oid |
| `ExpiresAt` | 過期時間 |
| `IsActive` | 啟用/停用金鑰 |
| `LastUsedAt` | 最後驗證時間 |
| `IsExpired` | 計算屬性：是否已過期 |
| `IsValid` | 計算屬性：`IsActive && !IsExpired` |

## API Key 格式

```
cak_<base64-url-safe-random-bytes>
```

- 前綴：`cak_`（Cundi API Key）
- 內容：32 位元組（256 位元）編碼為 URL 安全的 Base64

## 專案結構

```
Cundi.XAF.ApiKey/
├── BusinessObjects/
│   └── ApiKeyInfo.cs          # API Key 實體
├── Controllers/
│   └── ApiKeyViewController.cs # 產生/撤銷 Action
├── Parameters/
│   ├── ApiKeyGenerationParameters.cs  # 過期選擇
│   └── ApiKeyResultDisplay.cs         # 金鑰顯示彈出視窗
├── Services/
│   ├── ApiKeyGenerator.cs     # 金鑰生成
│   └── ApiKeyValidator.cs     # 金鑰驗證
└── ApiKeyModule.cs
```

## 安全性

- API Key **永不以明文儲存**
- 只有 SHA256 雜湊值儲存在資料庫中
- 使用加密安全的亂數產生器生成金鑰

## 授權

MIT License
