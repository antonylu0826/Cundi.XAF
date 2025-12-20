# Cundi.XAF.Triggers.Api

[Cundi.XAF.Triggers](../Cundi.XAF.Triggers/README_zh-TW.md) 的 WebApi 整合模組。

## 概要

此模組提供 `TriggerDataServicePlugin` - 一個 DataService 插件，用以在 ASP.NET Core WebApi 應用程式中啟用觸發器處理。它使用來自 [Cundi.XAF.Core.Api](../Cundi.XAF.Core.Api/README_zh-TW.md) 的可擴展 DataService 架構。

## 功能

- **Plugin 架構**：與 `CompositeDataService` 整合，提供可擴展性
- **同步執行**：Webhook 呼叫會在請求生命週期內同步執行
- **完整回應記錄**：將完整的 HTTP 回應詳細資訊記錄在 TriggerLog 中
- **Scoped 狀態管理**：處理並發請求的執行緒安全觸發器狀態

## 安裝

1. 新增專案參考：
   ```xml
   <ItemGroup>
     <ProjectReference Include="..\Modules\Cundi.XAF.Core.Api\Cundi.XAF.Core.Api.csproj" />
     <ProjectReference Include="..\Modules\Cundi.XAF.Triggers\Cundi.XAF.Triggers.csproj" />
     <ProjectReference Include="..\Modules\Cundi.XAF.Triggers.Api\Cundi.XAF.Triggers.Api.csproj" />
   </ItemGroup>
   ```

2. 在 `Startup.cs` 中註冊：
   ```csharp
   using Cundi.XAF.Triggers.Extensions;
   using Cundi.XAF.Core.Api.Extensions;
   
   // 註冊 Triggers 插件
   services.AddTriggers();
   
   // 註冊 CompositeDataService（必須在所有插件之後呼叫）
   services.AddCompositeDataService();
   ```

## 運作方式

```
API Request → CompositeDataService.CreateObjectSpace()
                           ↓
              TriggerDataServicePlugin.OnObjectSpaceCreated()
                           ↓
              ObjectSpace.Committing Event
              - 驗證物件
              - 擷取物件狀態
                           ↓
              ObjectSpace.Committed Event
              - 執行 webhooks（同步）
              - 記錄完整回應
                           ↓
              API Response ← Return to client
```

## 需求

- .NET 8.0+
- DevExpress XAF WebApi 24.2+
- Cundi.XAF.Core.Api 模組
- Cundi.XAF.Triggers 模組

## 授權

MIT License
