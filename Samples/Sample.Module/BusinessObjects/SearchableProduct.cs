using Cundi.XAF.FullTextSearch.Attributes;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

namespace Sample.Module.BusinessObjects;

/// <summary>
/// Test entity for FullTextSearch module.
/// Represents a searchable product with name, description, and specification fields.
/// Used to demonstrate global search functionality across multiple text properties.
/// </summary>
[DefaultClassOptions]
[GlobalSearchable]
public class SearchableProduct : BaseObject
{
    public SearchableProduct(Session session) : base(session) { }

    private string _name;
    /// <summary>
    /// Product name - indexed for full-text search.
    /// </summary>
    public string Name
    {
        get => _name;
        set => SetPropertyValue(nameof(Name), ref _name, value);
    }

    private string _description;
    /// <summary>
    /// Product description - indexed for full-text search.
    /// </summary>
    public string Description
    {
        get => _description;
        set => SetPropertyValue(nameof(Description), ref _description, value);
    }

    private string _specification;
    /// <summary>
    /// Product specification - large text field indexed for full-text search.
    /// </summary>
    [Size(SizeAttribute.Unlimited)]
    public string Specification
    {
        get => _specification;
        set => SetPropertyValue(nameof(Specification), ref _specification, value);
    }
}
