namespace Cundi.XAF.FullTextSearch.Attributes;

/// <summary>
/// Specifies whether a class should be included in global full-text search.
/// By default, all persistent types are searchable. Use [GlobalSearchable(false)] to exclude a type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class GlobalSearchableAttribute : Attribute
{
    /// <summary>
    /// Gets a value indicating whether this type should be included in global search.
    /// </summary>
    public bool IsSearchable { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalSearchableAttribute"/> class.
    /// </summary>
    /// <param name="isSearchable">Set to false to exclude this type from global search. Default is true.</param>
    public GlobalSearchableAttribute(bool isSearchable = true)
    {
        IsSearchable = isSearchable;
    }
}
