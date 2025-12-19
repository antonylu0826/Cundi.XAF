# DevExpress XAF 本地化完整指南

> 本指南適用於 DevExpress XAF 應用程式的全面本地化，包含業務物件、列舉、訊息和程式碼的本地化實作。

---

## 📋 目錄

1. [概述](#概述)
2. [XAF 本地化架構](#xaf-本地化架構)
3. [實作步驟](#實作步驟)
4. [檔案結構](#檔案結構)
5. [程式碼規範](#程式碼規範)
6. [常見模式](#常見模式)
7. [測試檢查清單](#測試檢查清單)
8. [常見問題](#常見問題)

---

## 概述

### 本地化範圍

XAF 應用程式的本地化包含以下元素：

- **業務物件**：類別和屬性的 Caption
- **列舉**：列舉值的顯示名稱
- **訊息**：Logger 訊息、UI 訊息、錯誤訊息
- **操作**：SimpleAction 的 Caption 和 ToolTip

### 支援的語言

- **預設語言**: 英文（存放在 [Model.DesignedDiffs.xafml](Model.DesignedDiffs.xafml)）
- **其他語言**: 繁體中文、簡體中文等（存放在 `Model.DesignedDiffs.Localization.{language-code}.xafml`），請注意，需要將此檔案的建置動作設定為`內嵌資源`，否則 XAF 會無法找到該檔案。

---

## XAF 本地化架構

### 核心概念

1. **Model.DesignedDiffs.xafml**: 存放預設語言（通常是英文）的所有設定
2. **Model.DesignedDiffs.Localization.{code}.xafml**: 存放特定語言的翻譯
3. **CaptionHelper.GetLocalizedText()**: 在程式碼中取得本地化文字的 API

### 語言代碼

| 語言 | 代碼 |
|------|------|
| 英文（預設） | 無特定檔案，使用 Model.DesignedDiffs.xafml |
| 繁體中文（台灣） | zh-Hant-TW |
| 簡體中文（中國） | zh-Hans-CN |
| 日文 | ja-JP |
| 韓文 | ko-KR |

---

## 實作步驟

### 步驟 1: 分析現有程式碼

#### 1.1 識別需要本地化的元素

使用 grep 搜尋硬編碼的文字：

```bash
# 搜尋 Logger 訊息
grep -r "LogInformation\|LogWarning\|LogError" --include="*.cs"

# 搜尋 UI 訊息
grep -r "ShowMessage" --include="*.cs"

# 搜尋硬編碼中文字串
grep -r "[\u4e00-\u9fff]" --include="*.cs"
```

#### 1.2 分類訊息

將找到的訊息分類到不同群組：

- **業務邏輯訊息**: Services 層的訊息
- **UI 訊息**: Controllers 的訊息
- **API 訊息**: API Controllers 的訊息
- **錯誤訊息**: Exception 和錯誤處理的訊息

### 步驟 2: 建立模型檔案

#### 2.1 更新 Model.DesignedDiffs.xafml (英文)

```xml
<?xml version="1.0" encoding="utf-8"?>
<Application>
  <!-- 業務物件本地化 -->
  <BOModel>
    <Class Name="YourNamespace.YourBusinessObject">
      <OwnMembers>
        <Member Name="PropertyName" Caption="Property Display Name" />
      </OwnMembers>
    </Class>
  </BOModel>
  
  <!-- 訊息本地化 -->
  <Localization>
    <!-- 列舉本地化 -->
    <LocalizationGroup Name="Enums">
      <LocalizationGroup Name="YourNamespace.YourEnum" Value="Enum Display Name">
        <LocalizationItem Name="EnumValue1" Value="Display Name 1" />
        <LocalizationItem Name="EnumValue2" Value="Display Name 2" />
      </LocalizationGroup>
    </LocalizationGroup>
    
    <!-- 自訂訊息 -->
    <LocalizationGroup Name="Messages">
      <LocalizationGroup Name="YourModuleFullName" IsNewNode="True">
        <!-- 按功能分組 -->
        <LocalizationItem Name="MessageKey" Value="English message with {Parameter}" IsNewNode="True" />
      </LocalizationGroup>
    </LocalizationGroup>
  </Localization>
</Application>
```

#### 2.2 建立 Model.DesignedDiffs.Localization.zh-Hant-TW.xafml (繁體中文)

```xml
<?xml version="1.0" encoding="utf-8"?>
<Application>
  <BOModel>
    <Class Name="YourNamespace.YourBusinessObject" Caption="業務物件顯示名稱">
      <OwnMembers>
        <Member Name="PropertyName" Caption="屬性顯示名稱" />
      </OwnMembers>
    </Class>
  </BOModel>
  
  <Localization>
    <LocalizationGroup Name="Enums">
      <LocalizationGroup Name="YourNamespace.YourEnum" Value="列舉顯示名稱">
        <LocalizationItem Name="EnumValue1" Value="顯示名稱 1" />
        <LocalizationItem Name="EnumValue2" Value="顯示名稱 2" />
      </LocalizationGroup>
    </LocalizationGroup>
    
    <LocalizationGroup Name="Messages">
      <LocalizationGroup Name="YourModuleFullName">
        <LocalizationItem Name="MessageKey" Value="繁體中文訊息 {參數}" />
      </LocalizationGroup>
    </LocalizationGroup>
  </Localization>
</Application>
```

### 步驟 3: 更新程式碼

#### 3.1 新增 Using 語句

在所有需要本地化的檔案中新增：

```csharp
using DevExpress.ExpressApp.Utils;
```

#### 3.2 替換硬編碼訊息

**之前**:
```csharp
_logger.LogInformation("開始執行作業，ID: {Id}", operationId);
```

**之後**:
```csharp
_logger.LogInformation(
    CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "StartOperation"), 
    operationId
);
```

### 步驟 4: 訊息命名規範

#### 4.1 命名原則

- 使用 **PascalCase**
- 名稱要**描述性強**，清楚表達訊息用途
- 按**動作類型**命名：
  - Start + 動作: `StartExecutingScript`
  - Error + 動作: `ErrorCompilingScript`
  - 完成狀態: `ExecutionCompleted`
  - 找不到: `ScriptNotFound`

#### 4.2 命名範例

```
✅ 好的命名：
- StartExecutingScript
- ErrorCompilingScript  
- ScriptNotFound
- CompilationSuccessful
- ExecutionTimeout

❌ 不好的命名：
- Message1
- Error
- Success
- Info
```

### 步驟 5: 處理訊息參數

#### 5.1 Logger 自動參數

Logger 會自動處理 `{參數名稱}` 格式：

```csharp
// Model.DesignedDiffs.xafml
<LocalizationItem Name="StartOperation" Value="Starting operation, ID: {OperationId}" />

// 程式碼
_logger.LogInformation(
    CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "StartOperation"),
    operationId  // Logger 會自動替換 {OperationId}
);
```

#### 5.2 手動參數替換

對於非 Logger 的訊息，使用 `.Replace()`:

```csharp
// Model.DesignedDiffs.xafml
<LocalizationItem Name="ExecutionTimeout" Value="Execution timeout (exceeded {Seconds} seconds)" />

// 程式碼
Message = CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "ExecutionTimeout")
    .Replace("{Seconds}", timeoutSeconds.ToString())
```

---

## 檔案結構

### 標準 XAF 模組結構

```
YourModule/
├── Model.DesignedDiffs.xafml                           # 預設語言（英文）
├── Model.DesignedDiffs.Localization.zh-Hant-TW.xafml  # 繁體中文
├── BusinessObjects/
│   ├── YourBusinessObject.cs
│   └── YourEnum.cs
├── Services/
│   └── YourService.cs                                 # 需要本地化
├── Controllers/
│   └── YourController.cs                              # 需要本地化
└── API/
    └── YourApiController.cs                           # 需要本地化
```

---

## 程式碼規範

### 1. Logger 訊息本地化

```csharp
// ❌ 錯誤：硬編碼
_logger.LogInformation("操作開始");

// ✅ 正確：使用本地化
_logger.LogInformation(
    CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "OperationStarted")
);

// ✅ 正確：帶參數
_logger.LogInformation(
    CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "OperationStarted"),
    operationId,
    userName
);
```

### 2. UI 訊息本地化

```csharp
// ❌ 錯誤：硬編碼
Application.ShowViewStrategy.ShowMessage(
    "請先輸入資料",
    InformationType.Warning
);

// ✅ 正確：使用本地化
Application.ShowViewStrategy.ShowMessage(
    CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "PleaseEnterData"),
    InformationType.Warning
);
```

### 3. XAF Action 本地化（推薦方法）

XAF Action 的 Caption 和 ToolTip 應該在模型檔案中定義，而不是在程式碼中硬編碼。

#### 3.1 在模型檔案中定義 Action

**Model.DesignedDiffs.xafml** (英文):
```xml
<?xml version="1.0" encoding="utf-8"?>
<Application>
  <ActionDesign>
    <Actions>
      <Action Id="CompileScript" Caption="Compile Script" ToolTip="Compile the current C# code" IsNewNode="True" />
      <Action Id="UpdateSchedule" Caption="Update Schedule" ToolTip="Update Hangfire schedule task" IsNewNode="True" />
    </Actions>
  </ActionDesign>
</Application>
```

**Model.DesignedDiffs.Localization.zh-Hant-TW.xafml** (繁中):
```xml
<?xml version="1.0" encoding="utf-8"?>
<Application>
  <ActionDesign>
    <Actions>
      <Action Id="CompileScript" Caption="編譯程式碼" ToolTip="編譯當前的 C# 程式碼" />
      <Action Id="UpdateSchedule" Caption="更新排程" ToolTip="更新 Hangfire 排程任務" />
    </Actions>
  </ActionDesign>
</Application>
```

#### 3.2 在程式碼中建立 Action

在 Controller 中，只需定義 Action ID，Caption 和 ToolTip 會自動從模型檔案載入：

```csharp
public class ScriptCompilationController : ObjectViewController<DetailView, ScriptDefinition>
{
    private SimpleAction _compileAction;

    public ScriptCompilationController() 
    {
        // 只需定義 ID，Caption 和 ToolTip 由模型檔案提供
        _compileAction = new SimpleAction(this, "CompileScript", "Tools")
        {
            ImageName = "Action_Refresh"
        };
        
        _compileAction.Execute += CompileAction_Execute;
    }
    
    // ... 其他程式碼
}
```

#### 3.3 程式碼中本地化的替代方法（不推薦）

如果確實需要在程式碼中動態設定，可以使用 `CaptionHelper`：

```csharp
// ⚠️ 不推薦：應該使用模型檔案
_myAction = new SimpleAction(this, "MyAction", "Tools")
{
    Caption = CaptionHelper.GetLocalizedText(@"Actions", "MyActionCaption"),
    ToolTip = CaptionHelper.GetLocalizedText(@"Actions", "MyActionTooltip")
};
```

**為什麼推薦使用模型檔案？**
1. **集中管理**: 所有 Action 的本地化定義都在模型檔案中
2. **XAF 標準**: 符合 XAF 的標準做法
3. **Model Editor**: 可以使用 XAF Model Editor 視覺化管理
4. **程式碼簡潔**: Controller 程式碼更簡潔，只關注邏輯

### 4. Exception 訊息本地化

```csharp
// ❌ 錯誤：硬編碼
throw new InvalidOperationException("找不到資料");

// ✅ 正確：使用本地化
throw new InvalidOperationException(
    CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "DataNotFound")
);
```

---

## 常見模式

### 模式 1: Service 層訊息

```csharp
public class YourService
{
    private readonly ILogger<YourService> _logger;
    
    public async Task<Result> ProcessAsync(Guid id)
    {
        try
        {
            _logger.LogInformation(
                CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "StartProcessing"),
                id
            );
            
            // 處理邏輯...
            
            _logger.LogInformation(
                CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "ProcessingCompleted"),
                id
            );
            
            return new Result 
            { 
                Success = true,
                Message = CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "ProcessSuccess")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "ErrorProcessing"),
                id
            );
            
            return new Result
            {
                Success = false,
                Message = CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "ProcessFailed")
                    .Replace("{Error}", ex.Message)
            };
        }
    }
}
```

### 模式 2: Controller 訊息

```csharp
public class YourController : ObjectViewController<DetailView, YourObject>
{
    private async void Action_Execute(object sender, SimpleActionExecuteEventArgs e)
    {
        if (View.CurrentObject is not YourObject obj)
            return;
            
        try
        {
            if (string.IsNullOrWhiteSpace(obj.RequiredField))
            {
                Application.ShowViewStrategy.ShowMessage(
                    CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "FieldRequired"),
                    InformationType.Warning
                );
                return;
            }
            
            Application.ShowViewStrategy.ShowMessage(
                CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "ProcessingPleaseWait"),
                InformationType.Info
            );
            
            var result = await _service.ProcessAsync(obj.Oid);
            
            Application.ShowViewStrategy.ShowMessage(
                result.Message,
                result.Success ? InformationType.Success : InformationType.Error
            );
        }
        catch (Exception ex)
        {
            Application.ShowViewStrategy.ShowMessage(
                CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "UnexpectedError")
                    .Replace("{Error}", ex.Message),
                InformationType.Error
            );
        }
    }
}
```

### 模式 3: API Controller 訊息

```csharp
[ApiController]
[Route("api/[controller]")]
public class YourApiController : ControllerBase
{
    private readonly ILogger<YourApiController> _logger;
    
    [HttpPost("{id}/process")]
    public async Task<IActionResult> Process(Guid id)
    {
        try
        {
            _logger.LogInformation(
                CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "ApiProcessingStarted"),
                id
            );
            
            var result = await _service.ProcessAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(new ErrorResponse
                {
                    Error = CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "ProcessingFailed"),
                    Details = result.Message
                });
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "ApiProcessingError"),
                id
            );
            
            return StatusCode(500, new ErrorResponse
            {
                Error = CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "InternalError"),
                Details = ex.Message
            });
        }
    }
}
```

---

## 測試檢查清單

### 編譯階段

- [ ] 所有檔案都已新增 `using DevExpress.ExpressApp.Utils;`
- [ ] 所有 `CaptionHelper.GetLocalizedText` 呼叫語法正確
- [ ] 專案編譯無錯誤
- [ ] 無 CaptionHelper 找不到的錯誤

### 模型檔案檢查

- [ ] Model.DesignedDiffs.xafml 包含所有英文訊息
- [ ] Model.DesignedDiffs.Localization.zh-Hant-TW.xafml 包含所有繁中翻譯
- [ ] 訊息 Key 在兩個檔案中完全一致
- [ ] 參數格式 `{ParameterName}` 在兩個檔案中一致
- [ ] XML 格式正確，無語法錯誤

### 執行時測試

#### 1. 語言切換測試

- [ ] 切換到繁體中文，所有訊息顯示繁中
- [ ] 切換到英文，所有訊息顯示英文
- [ ] 切換語言後重新啟動應用程式，語言設定保持

#### 2. 功能測試

針對每個功能模組：

- [ ] **正常流程**: 所有成功訊息正確顯示
- [ ] **錯誤處理**: 所有錯誤訊息正確顯示
- [ ] **驗證訊息**: 所有驗證失敗訊息正確顯示
- [ ] **進度訊息**: 所有進度提示訊息正確顯示

#### 3. Logger 測試

- [ ] 查看 Log 檔案，確認 Logger 訊息已本地化
- [ ] 確認參數正確替換到訊息中
- [ ] 確認不同 Log Level 的訊息都已本地化

#### 4. UI 測試

- [ ] SimpleAction 的 Caption 和 ToolTip 已本地化
- [ ] ShowMessage 訊息已本地化
- [ ] 業務物件的 Caption 已本地化
- [ ] 列舉值的顯示名稱已本地化

---

## 常見問題

### Q1: CaptionHelper 找不到

**原因**: 缺少 using 語句

**解決方案**:
```csharp
using DevExpress.ExpressApp.Utils;
```

### Q2: 訊息沒有本地化

**可能原因**:
1. 訊息 Key 在模型檔案中不存在
2. 訊息 Key 拼寫錯誤
3. 語言設定不正確

**解決方案**:
1. 檢查 [Model.DesignedDiffs.xafml](file:///c:/Users/anthony/Desktop/Cundi/CundiApi.Scripts/Model.DesignedDiffs.xafml) 是否有該 Key
2. 檢查 [Model.DesignedDiffs.Localization.zh-Hant-TW.xafml](file:///c:/Users/anthony/Desktop/Cundi/CundiApi.Scripts/Model.DesignedDiffs.Localization.zh-Hant-TW.xafml) 是否有對應翻譯
3. 確認應用程式語言設定正確

### Q3: 參數沒有正確替換

**Logger 參數**:
```csharp
// ✅ 正確 - Logger 會自動處理
_logger.LogInformation(
    CaptionHelper.GetLocalizedText(@"Messages\Module", "MessageKey"),
    param1, param2
);
```

**一般訊息參數**:
```csharp
// ✅ 正確 - 需要手動 Replace
Message = CaptionHelper.GetLocalizedText(@"Messages\Module", "MessageKey")
    .Replace("{Param1}", value1)
    .Replace("{Param2}", value2)
```

### Q4: 模型檔案格式錯誤

**確保**:
- XML 格式正確（使用 XML 編輯器驗證）
- 所有標籤正確關閉
- IsNewNode 屬性僅用於新增項目
- 編碼為 UTF-8

### Q5: 如何新增新的語言

1. 建立新的本地化檔案：`Model.DesignedDiffs.Localization.{language-code}.xafml`
2. 複製 [Model.DesignedDiffs.Localization.zh-Hant-TW.xafml](file:///c:/Users/anthony/Desktop/Cundi/CundiApi.Scripts/Model.DesignedDiffs.Localization.zh-Hant-TW.xafml) 的結構
3. 翻譯所有 Value 屬性為目標語言
4. 在應用程式中啟用該語言

---

## 最佳實踐總結

### ✅ 應該做的

1. **集中管理訊息**: 所有訊息都定義在模型檔案中
2. **語意化命名**: 使用描述性的訊息 Key
3. **參數一致性**: 確保參數名稱在所有語言中一致
4. **分組管理**: 按功能模組分組訊息
5. **完整測試**: 測試所有語言和所有功能
6. **文件記錄**: 記錄所有本地化訊息的用途

### ❌ 不應該做的

1. **硬編碼文字**: 任何使用者可見的文字都不應硬編碼
2. **混合語言**: 不要在一個訊息中混用多種語言
3. **重複訊息**: 相同意義的訊息應該重用
4. **忽略 Logger**: Logger 訊息也需要本地化
5. **缺少翻譯**: 每個訊息都應該有對應的翻譯
6. **忽略參數**: 忘記在翻譯中保留參數佔位符

---

## 快速開始範本

### 1. 建立基本結構

```xml
<!-- Model.DesignedDiffs.xafml -->
<?xml version="1.0" encoding="utf-8"?>
<Application>
  <Localization>
    <LocalizationGroup Name="Messages">
      <LocalizationGroup Name="YourModuleFullName" IsNewNode="True">
        <!-- 在這裡新增訊息 -->
      </LocalizationGroup>
    </LocalizationGroup>
  </Localization>
</Application>
```

```xml
<!-- Model.DesignedDiffs.Localization.zh-Hant-TW.xafml -->
<?xml version="1.0" encoding="utf-8"?>
<Application>
  <Localization>
    <LocalizationGroup Name="Messages">
      <LocalizationGroup Name="YourModuleFullName">
        <!-- 在這裡新增翻譯 -->
      </LocalizationGroup>
    </LocalizationGroup>
  </Localization>
</Application>
```

### 2. 新增第一個訊息

**英文**:
```xml
<LocalizationItem Name="WelcomeMessage" Value="Welcome to the application" IsNewNode="True" />
```

**繁中**:
```xml
<LocalizationItem Name="WelcomeMessage" Value="歡迎使用本應用程式" />
```

### 3. 在程式碼中使用

```csharp
using DevExpress.ExpressApp.Utils;

// 使用訊息
var message = CaptionHelper.GetLocalizedText(@"Messages\YourModuleFullName", "WelcomeMessage");
Application.ShowViewStrategy.ShowMessage(message, InformationType.Info);
```

---

## 版本記錄

- **v1.0** (2025-12-05): 初始版本，基於 CundiApi.Scripts 模組本地化經驗

---

## 授權

本指南基於 CundiApi.Scripts 專案的實際實作經驗整理而成，可自由使用於任何 DevExpress XAF 專案。
