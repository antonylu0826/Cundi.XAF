# Cundi.XAF

此方案專注於擴充 XAF 框架的 Triggers 功能。

## 模組 (Modules)

核心功能位於 `Modules` 目錄下：

*   **Cundi.XAF.Triggers**: 核心 Triggers 模組，提供觸發器相關的邏輯與實作。
*   **Cundi.XAF.Triggers.Api**: Triggers 的 API 整合模組，支援 Web API 環境下的觸發器運作。

## 範例 (Samples)

各個平台的實作範例位於 `Samples` 目錄下：

*   **Sample.Win**: Windows Forms 應用程式範例。
*   **Sample.Blazor.Server**: Blazor Server 應用程式範例。
*   **Sample.WebApi**: Web API 應用程式範例。

## 如何開始 (Getting Started)

1.  使用 Visual Studio 開啟 `Cundi.XAF.sln` 方案檔。
2.  還原 NuGet 套件。
3.  將您想要執行的範例專案 (例如 `Sample.Blazor.Server`) 設定為起始專案。
4.  建置並執行方案。
