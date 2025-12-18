namespace Cundi.XAF.FullTextSearch.Attributes;

/// <summary>
/// Specifies whether a property should be included in global full-text search.
/// By default, all string properties are searchable. Use [GlobalSearchableProperty(false)] to exclude a property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class GlobalSearchablePropertyAttribute : Attribute
{
    /// <summary>
    /// Gets a value indicating whether this property should be included in global search.
    /// </summary>
    public bool IsSearchable { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalSearchablePropertyAttribute"/> class.
    /// </summary>
    /// <param name="isSearchable">Set to false to exclude this property from global search. Default is true.</param>
    public GlobalSearchablePropertyAttribute(bool isSearchable = true)
    {
        IsSearchable = isSearchable;
    }
}
