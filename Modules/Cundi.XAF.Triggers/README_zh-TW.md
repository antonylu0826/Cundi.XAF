# Cundi.XAF.Triggers

一個 DevExpress XAF 模組，用於偵測物件變更並根據可設定的規則觸發外部 Webhook。

## 功能

- **物件變更偵測 (Object Change Detection)**：監控 XAF 業務物件的建立 (Create)、修改 (Modify) 和刪除 (Delete) 操作
- **觸發規則 (Trigger Rules)**：定義規則以指定哪些物件類型和事件應觸發 Webhook
- **彈性的 HTTP 方法 (Flexible HTTP Methods)**：支援 POST、GET、PUT、PATCH 和 DELETE 方法
- **執行記錄 (Execution Logging)**：追蹤所有觸發執行，包含詳細的狀態和回應資訊
- **非同步執行 (Async Execution) (WinForms/Blazor)**：不阻塞 UI 效能的 Webhook 呼叫
- **同步執行 (Sync Execution) (WebApi)**：確保在請求生命週期內正確記錄
- **硬刪除模式 (Hard Delete Mode)**：全域停用 XPO 的延遲刪除 (soft delete)
- **清除記錄動作 (Clear Logs Action)**：一鍵清除特定規則的所有記錄

## 架構

此模組支援兩種執行環境：

| 環境 context | 控制器/服務 Controller/Service | 執行模式 Execution Mode | 記錄 Logging |
|---|---|---|---|
| WinForms/Blazor | `TriggerWindowController` | 非同步 (射後不理) | 執行前記錄 |
| WebApi | `WebApiMiddleDataService` | 同步 | 完整回應記錄 |

## 安裝

### WinForms / Blazor Desktop

1.  新增專案參考至您的 XAF 應用程式：
    ```xml
    <ItemGroup>
      <ProjectReference Include="..\Modules\Cundi.XAF.Triggers\Cundi.XAF.Triggers.csproj" />
    </ItemGroup>
    ```

2.  將模組新增至您的應用程式 `Module.cs`：
    ```csharp
    RequiredModuleTypes.Add(typeof(Cundi.XAF.Triggers.TriggersModule));
    ```

### WebApi

1.  新增兩個專案參考：
    ```xml
    <ItemGroup>
      <ProjectReference Include="..\Modules\Cundi.XAF.Triggers\Cundi.XAF.Triggers.csproj" />
      <ProjectReference Include="..\Modules\Cundi.XAF.Triggers.Api\Cundi.XAF.Triggers.Api.csproj" />
    </ItemGroup>
    ```

2.  在 `Startup.cs` 中註冊自訂 DataService：
    ```csharp
    using Cundi.XAF.Triggers.Api;
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IDataService, WebApiMiddleDataService>();
        // ... 其他服務
    }
    ```

3.  執行應用程式以更新資料庫結構。

## 使用方式

### 建立觸發規則 (Creating a Trigger Rule)

1.  導覽至 XAF 應用程式中的 **Trigger Rules**。
2.  建立新規則並設定：
    - **Name**：規則的描述性名稱
    - **Target Type**：從下拉選單選擇要監控的物件類型
    - **OnCreated**：勾選以在物件建立時觸發
    - **OnModified**：勾選以在物件修改時觸發
    - **OnRemoved**：勾選以在物件刪除時觸發
    - **HttpMethod**：選擇 HTTP 方法 (預設：POST)
    - **Webhook URL**：接收通知的端點
    - **Is Active**：啟用/停用規則

### 測試 Webhook (Testing Webhooks)

在 TriggerRule 詳細檢視中，點擊 **Test Webhook** 按鈕以發送測試請求至設定的 URL。測試 payload 包含 `isTest: true` 標記。

### 清除記錄 (Clearing Logs)

在 TriggerRule 詳細檢視中，點擊 **Clear Logs** 按鈕以刪除該規則相關的所有觸發記錄。

### 記錄保留策略 (Log Retention Policy)

舊的觸發記錄會在應用程式啟動時自動清理。預設情況下，超過 **90 天** 的記錄將被刪除。

### Webhook Payload 格式

當觸發時，模組會發送一個 HTTP 請求，其 JSON 結構如下：

```json
{
  "eventType": "Created",
  "objectType": "YourNamespace.Customer",
  "objectKey": "abc123-def456",
  "timestamp": "2024-12-17T16:00:00.000Z",
  "triggerRule": "Customer Created Notification",
  "data": {
    "Name": "John Doe",
    "Email": "john@example.com"
  }
}
```

> **注意**：對於 `Deleted` 事件，由於物件已不再可用，`data` 欄位將為 `null`。

### 檢視觸發記錄 (Viewing Trigger Logs)

導覽至 **Trigger Logs** 查看執行歷史記錄，包含：
- 執行時間
- 使用的 HTTP 方法
- 成功/失敗狀態
- HTTP 狀態碼
- 回應內容 (Response body)
- 錯誤訊息 (如果有)

## 設定選項

### TriggerRule 屬性

| 屬性 Property | 類型 Type | 描述 Description |
|---|---|---|
| `Name` | string | 規則名稱 |
| `Description` | string | 選擇性描述 |
| `TargetType` | Type | 要監控的物件類型 (下拉選擇) |
| `OnCreated` | bool | 於物件建立時觸發 |
| `OnModified` | bool | 於物件修改時觸發 |
| `OnRemoved` | bool | 於物件刪除時觸發 |
| `IsActive` | bool | 啟用/停用規則 |
| `HttpMethod` | enum | HTTP 方法 (Post, Get, Put, Patch, Delete) |
| `WebhookUrl` | string | 目標 Webhook URL |
| `CustomHeaders` | string | 自訂 HTTP 標頭的 JSON 物件 |

### 自訂標頭範例

```json
{
  "Authorization": "Bearer your-token",
  "X-Custom-Header": "custom-value"
}
```

## 技術說明

### 物件變更偵測

模組使用 XAF 的 ObjectSpace 事件來偵測變更：

| 事件類型 Event Type | 偵測方法 Detection Method |
|---|---|
| Created | `Committing` 事件中的 `IsNewObject()` |
| Modified | `ModifiedObjects` 中的物件，且非新增也非刪除 |
| Deleted | `Committing` 事件中的 `GetObjectsToDelete()` |

### 執行流程

```
┌─────────────────────────────────────────────────────────────┐
│                    ObjectSpace.Committing                    │
├─────────────────────────────────────────────────────────────┤
│  1. 擷取物件狀態 (Created/Modified/Deleted)                 │
│  2. 比對生效的 TriggerRules                                 │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    ObjectSpace.Committed                     │
├─────────────────────────────────────────────────────────────┤
│  3. 建構 webhook payload                                    │
│  4. 執行 webhook (WinForms 為非同步，WebApi 為同步)         │
│  5. 記錄執行結果                                            │
└─────────────────────────────────────────────────────────────┘
```

### 延遲刪除 (Deferred Deletion)

此模組會自動停用 XPO 的延遲刪除 (soft delete) 功能。物件將從資料庫中永久刪除，而不是被標記 `GCRecord` 時間戳記。

## 需求

- .NET 8.0+
- DevExpress XAF 24.2+

## 授權

MIT License
