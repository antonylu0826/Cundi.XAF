using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using System.ComponentModel;

namespace Cundi.XAF.FullTextSearch.BusinessObjects;

/// <summary>
/// Non-persistent object representing a global search result.
/// Each instance points to a persistent object found during the search.
/// </summary>
[DomainComponent]
[NavigationItem(false)]
[DefaultProperty(nameof(DisplayName))]
[DevExpress.ExpressApp.DC.XafDefaultProperty(nameof(DisplayName))]
// Disable create/edit/delete
[CreatableItem(false)]
public class GlobalSearchResult : NonPersistentBaseObject
{
    /// <summary>
    /// Gets or sets the type of the target object.
    /// </summary>
    [Browsable(false)]  // Hide from ListView
    public Type? TargetObjectType { get; set; }

    /// <summary>
    /// Gets or sets the key value of the target object as a string.
    /// </summary>
    [Browsable(false)]  // Hide from ListView
    public string? TargetObjectKey { get; set; }

    /// <summary>
    /// Gets or sets the display name of the target object (usually from ToString() or DefaultProperty).
    /// </summary>
    [ModelDefault("Caption", "Name")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Gets or sets the type name in a human-readable format.
    /// </summary>
    [ModelDefault("Caption", "Type")]
    public string? TypeCaption { get; set; }

    /// <summary>
    /// Gets or sets a summary of where the match was found.
    /// </summary>
    [ModelDefault("Caption", "Matched Content")]
    [Size(SizeAttribute.Unlimited)]
    public string? MatchedContent { get; set; }
}
