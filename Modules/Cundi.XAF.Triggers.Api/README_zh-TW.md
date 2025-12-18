# Cundi.XAF.Triggers.Api

[Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README_zh-TW.md) 的 WebApi 整合模組。

## 概要

此模組提供 `WebApiMiddleDataService` - 一個自訂的 `IDataService` 實作，用以在 ASP.NET Core WebApi 應用程式中啟用觸發器處理。

## 功能

- **同步執行 (Sync Execution)**：Webhook 呼叫會在請求生命週期內同步執行
- **完整回應記錄 (Full Response Logging)**：將完整的 HTTP 回應詳細資訊記錄在 TriggerLog 中
- **Scoped 狀態管理 (Scoped State Management)**：處理並發請求的執行緒安全觸發器狀態

## 安裝

1. 新增專案參考：
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\Modules\Cundi.XAF.Triggers\Cundi.XAF.Triggers.csproj" />
     <ProjectReference Include="..\Modules\Cundi.XAF.Triggers.Api\Cundi.XAF.Triggers.Api.csproj" />
   </ItemGroup>
   ```

2. 在 `Startup.cs` 中註冊：
   ```csharp
   using Cundi.XAF.Triggers.Api;
   
   services.AddScoped<IDataService, WebApiMiddleDataService>();
   ```

## 運作方式

```
API Request → WebApiMiddleDataService.CreateObjectSpace()
                           ↓
              ObjectSpace.Committing Event
              - 擷取物件狀態 (Capture object states)
              - 儲存於執行個體欄位 (Scoped)
                           ↓
              ObjectSpace.Committed Event
              - 執行 webhooks (sync)
              - 記錄完整回應 (Log with full response)
                           ↓
              API Response ← Return to client
```

## 需求

- .NET 8.0+
- DevExpress XAF WebApi 24.2+
- Cundi.XAF.Triggers 模組

## 授權

MIT License
