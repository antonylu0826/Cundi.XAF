# Cundi.XAF.FullTextSearch

A global full-text search module for DevExpress XAF applications. Search across all business objects from a single entry point.

## Features

- **Global Search**: Search across all persistent business objects with a single keyword
- **Platform Support**: Works on both Blazor and WinForms platforms
- **Smart Placement**: Automatically positions the search action based on platform (QuickAccess for Blazor, Tools toolbar for WinForms)
- **Performance Safeguards**: Configurable limits per type and total results
- **Double-Click Navigation**: Click on a search result to open the target object

## Installation

Add the module reference to your XAF Module project:

```csharp
// In your Module.cs
RequiredModuleTypes.Add(typeof(Cundi.XAF.FullTextSearch.FullTextSearchModule));
```

## Usage

1. Launch your XAF application
2. Look for the **Global Search** action:
   - **Blazor**: Located in the header near the user icon
   - **WinForms**: Located in the Tools toolbar
3. Enter your search keyword and press Enter
4. Click on any result to navigate to that object

## Configuration

### Customize Search Limits

Modify `GlobalSearchService` properties:

```csharp
var searchService = new GlobalSearchService(objectSpace, typesInfo)
{
    MaxResultsPerType = 50,  // Max results per object type
    MaxTotalResults = 200    // Total max results
};
```

### Exclude Types from Search

Override `IsSystemType` in `GlobalSearchService` to customize which types are excluded.

### Control with Attributes

Use attributes to control which types and properties are included in global search:

```csharp
using Cundi.XAF.FullTextSearch.Attributes;

// Exclude an entire type from global search
[GlobalSearchable(false)]
public class InternalConfig : BaseObject
{
    // ...
}

// Exclude specific properties from search
public class Customer : BaseObject
{
    public string Name { get; set; }
    
    [GlobalSearchableProperty(false)]
    public string InternalNotes { get; set; }  // Won't be searched
}

## Architecture

| File | Description |
|------|-------------|
| `GlobalSearchResult.cs` | Non-persistent result object |
| `GlobalSearchService.cs` | Core search logic |
| `GlobalSearchController.cs` | Action and UI handling |
| `GlobalSearchResultNavigationController.cs` | Result navigation |

## Requirements

- DevExpress XAF 24.2+
- .NET 8.0+
