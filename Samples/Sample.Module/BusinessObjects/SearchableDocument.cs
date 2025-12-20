using Cundi.XAF.FullTextSearch.Attributes;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Sample.Module.BusinessObjects;

/// <summary>
/// Test entity for FullTextSearch module.
/// Represents a searchable document with name, subject, and note fields.
/// Used to demonstrate global search functionality in document-like content.
/// </summary>
[DefaultClassOptions]
[GlobalSearchable]
public class SearchableDocument : BaseObject
{
    public SearchableDocument(Session session) : base(session) { }

    private string _name;
    /// <summary>
    /// Document name - indexed for full-text search.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetPropertyValue(nameof(Name), ref _name, value);
    }

    private string _subject;
    /// <summary>
    /// Document subject - indexed for full-text search.
    /// </summary>
    public string Subject
    {
        get => _subject;
        set => SetPropertyValue(nameof(Subject), ref _subject, value);
    }

    private string _note;
    /// <summary>
    /// Document note - large text field indexed for full-text search.
    /// </summary>
    [Size(SizeAttribute.Unlimited)]
    public string Note
    {
        get => _note;
        set => SetPropertyValue(nameof(Note), ref _note, value);
    }
}
