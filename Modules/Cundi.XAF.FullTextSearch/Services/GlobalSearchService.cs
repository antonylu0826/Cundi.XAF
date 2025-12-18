using Cundi.XAF.FullTextSearch.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

namespace Cundi.XAF.FullTextSearch.Services;

/// <summary>
/// Service responsible for executing global full-text search across all persistent types.
/// Scans all visible persistent types and searches string properties for keyword matches.
/// </summary>
public class GlobalSearchService
{
    private readonly IObjectSpace _objectSpace;
    private readonly ITypesInfo _typesInfo;

    /// <summary>
    /// Maximum number of results per type to avoid performance issues. Default: 50.
    /// </summary>
    public int MaxResultsPerType { get; set; } = 50;

    /// <summary>
    /// Maximum total results to return. Default: 200.
    /// </summary>
    public int MaxTotalResults { get; set; } = 200;

    public GlobalSearchService(IObjectSpace objectSpace, ITypesInfo typesInfo)
    {
        _objectSpace = objectSpace;
        _typesInfo = typesInfo;
    }

    /// <summary>
    /// Executes a global search across all persistent types.
    /// </summary>
    /// <param name="keyword">The search keyword.</param>
    /// <returns>List of search results.</returns>
    public List<GlobalSearchResult> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return new List<GlobalSearchResult>();

        var results = new List<GlobalSearchResult>();
        var persistentTypes = GetSearchableTypes();

        foreach (var typeInfo in persistentTypes)
        {
            if (results.Count >= MaxTotalResults)
                break;

            try
            {
                var typeResults = SearchInType(typeInfo, keyword);
                results.AddRange(typeResults.Take(MaxTotalResults - results.Count));
            }
            catch
            {
                // Skip types that fail to query
                continue;
            }
        }

        return results;
    }

    /// <summary>
    /// Gets all searchable persistent types from the type system.
    /// </summary>
    private IEnumerable<ITypeInfo> GetSearchableTypes()
    {
        return _typesInfo.PersistentTypes
            .Where(t => t.IsVisible && t.IsPersistent && !t.IsAbstract)
            .Where(t => !IsSystemType(t))
            .ToList();
    }

    /// <summary>
    /// Checks if a type is a system type that should be excluded from search.
    /// </summary>
    private static bool IsSystemType(ITypeInfo typeInfo)
    {
        var typeName = typeInfo.FullName;
        return typeName.StartsWith("DevExpress.") ||
               typeName.StartsWith("XafSecurity") ||
               typeName.Contains("ModuleInfo") ||
               typeName.Contains("AuditData") ||
               typeName.Contains("ModelDifference");
    }

    /// <summary>
    /// Searches for the keyword within a specific type.
    /// </summary>
    private List<GlobalSearchResult> SearchInType(ITypeInfo typeInfo, string keyword)
    {
        var results = new List<GlobalSearchResult>();
        var criteria = BuildSearchCriteria(typeInfo, keyword);

        if (criteria is null)
            return results;

        var objects = _objectSpace.GetObjects(typeInfo.Type, criteria, true);
        var count = 0;

        foreach (var obj in objects)
        {
            if (count >= MaxResultsPerType)
                break;

            var keyValue = _objectSpace.GetKeyValue(obj);
            var displayName = GetDisplayName(obj, typeInfo);
            var matchedContent = GetMatchedContent(obj, typeInfo, keyword);

            results.Add(new GlobalSearchResult
            {
                TargetObjectType = typeInfo.Type,
                TargetObjectKey = keyValue?.ToString(),
                DisplayName = displayName,
                TypeCaption = typeInfo.Name,
                MatchedContent = matchedContent
            });

            count++;
        }

        return results;
    }

    /// <summary>
    /// Builds a criteria operator that searches all string properties for the keyword.
    /// </summary>
    private static CriteriaOperator? BuildSearchCriteria(ITypeInfo typeInfo, string keyword)
    {
        var stringProperties = typeInfo.Members
            .Where(m => m.MemberType == typeof(string))
            .Where(m => m.IsPersistent || m.IsPublic)
            .Where(m => !m.Name.Contains("Password") && !m.Name.Contains("Hash"))
            .Where(m => !m.Name.StartsWith("_"))
            .ToList();

        if (!stringProperties.Any())
            return null;

        var conditions = stringProperties
            .Select(m => new FunctionOperator(FunctionOperatorType.Contains,
                new OperandProperty(m.Name),
                new OperandValue(keyword)))
            .Cast<CriteriaOperator>()
            .ToList();

        return CriteriaOperator.Or(conditions.ToArray());
    }

    /// <summary>
    /// Gets the display name of an object using its DefaultProperty or ToString().
    /// </summary>
    private static string GetDisplayName(object obj, ITypeInfo typeInfo)
    {
        var defaultProperty = typeInfo.DefaultMember;
        if (defaultProperty != null)
        {
            var value = defaultProperty.GetValue(obj);
            if (value != null)
                return value.ToString() ?? obj.ToString() ?? typeInfo.Name;
        }

        return obj.ToString() ?? typeInfo.Name;
    }

    /// <summary>
    /// Gets a summary of the matched content for display.
    /// </summary>
    private static string GetMatchedContent(object obj, ITypeInfo typeInfo, string keyword)
    {
        var matchedProperties = new List<string>();

        foreach (var member in typeInfo.Members.Where(m => m.MemberType == typeof(string)))
        {
            try
            {
                var value = member.GetValue(obj)?.ToString();
                if (!string.IsNullOrEmpty(value) &&
                    value.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    var preview = value.Length > 50 ? value[..50] + "..." : value;
                    matchedProperties.Add($"{member.DisplayName ?? member.Name}: {preview}");
                }
            }
            catch
            {
                // Skip properties that fail to read
            }
        }

        return string.Join(" | ", matchedProperties.Take(3));
    }
}
