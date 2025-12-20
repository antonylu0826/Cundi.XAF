# Cundi.XAF.Core.Api

提供可擴展 DataService Plugin 架構的核心 WebApi 基礎設施模組。

## 概要

此模組提供 `CompositeDataService` 和 `IDataServicePlugin` - 一個可擴展的架構，允許多個模組擴展 DataService 行為而不會發生衝突。

## 功能

- **Plugin 架構**：多個模組可以註冊插件，在 ObjectSpace 建立時全部執行
- **順序執行**：插件根據其 `Order` 屬性依序執行
- **無衝突**：解決了 DI 容器只有最後註冊的 `IDataService` 生效的限制

## 元件

| 類別 | 說明 |
| --- | --- |
| `IDataServicePlugin` | 插件需實作的介面 |
| `CompositeDataService` | 委派給所有已註冊插件的 DataService |
| `CoreApiExtensions` | 服務註冊的擴展方法 |

## 安裝

1. 新增專案參考：
   ```xml
   <ProjectReference Include="path\to\Cundi.XAF.Core.Api\Cundi.XAF.Core.Api.csproj" />
   ```

2. 在 `Startup.cs` 中註冊：
   ```csharp
   using Cundi.XAF.Core.Api.Extensions;
   
   // 先註冊模組的插件
   services.AddDataServicePlugin<YourPlugin>();
   
   // 然後註冊 CompositeDataService（必須放在最後）
   services.AddCompositeDataService();
   ```

## 建立插件

```csharp
public class YourPlugin : IDataServicePlugin
{
    // 數值越小越先執行
    public int Order => 50;
    
    public void OnObjectSpaceCreated(IObjectSpace objectSpace, Type objectType)
    {
        // 訂閱 ObjectSpace 事件
        objectSpace.Committing += (s, e) => { /* 您的邏輯 */ };
        objectSpace.Committed += (s, e) => { /* 您的邏輯 */ };
    }
}
```

## 內建插件

| 模組 | 插件 | Order | 說明 |
| --- | --- | --- | --- |
| Cundi.XAF.DataMirror.Api | `MirroredObjectDataServicePlugin` | 0 | 阻擋對 MirroredObject 類型的修改 |
| Cundi.XAF.Triggers.Api | `TriggerDataServicePlugin` | 100 | 處理觸發規則並執行 webhooks |

## 需求

- .NET 8.0+
- DevExpress XAF WebApi 24.2+

## 授權

MIT License
