namespace Cundi.XAF.FullTextSearch.Api.DTOs;

/// <summary>
/// DTO representing a single search result.
/// </summary>
public class GlobalSearchResultDto
{
    /// <summary>
    /// Display name of the matched object.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Key value of the matched object.
    /// </summary>
    public string? ObjectKey { get; set; }

    /// <summary>
    /// Human-readable type name.
    /// </summary>
    public string? TypeCaption { get; set; }

    /// <summary>
    /// Full type name including namespace.
    /// </summary>
    public string? TypeFullName { get; set; }

    /// <summary>
    /// Summary of where the keyword was matched.
    /// </summary>
    public string? MatchedContent { get; set; }
}

/// <summary>
/// DTO for global search API response.
/// </summary>
public class GlobalSearchResponseDto
{
    /// <summary>
    /// The search keyword that was used.
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// Total number of results returned.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// List of search results.
    /// </summary>
    public List<GlobalSearchResultDto> Results { get; set; } = new();
}
