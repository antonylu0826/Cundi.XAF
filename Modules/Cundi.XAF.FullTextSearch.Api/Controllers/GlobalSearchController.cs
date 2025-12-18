using Cundi.XAF.FullTextSearch.Api.DTOs;
using Cundi.XAF.FullTextSearch.Services;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cundi.XAF.FullTextSearch.Api.Controllers;

/// <summary>
/// API controller providing global full-text search functionality.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GlobalSearchController : ControllerBase
{
    private readonly INonSecuredObjectSpaceFactory _nonSecuredObjectSpaceFactory;
    private readonly ITypesInfo _typesInfo;

    public GlobalSearchController(
        INonSecuredObjectSpaceFactory nonSecuredObjectSpaceFactory,
        ITypesInfo typesInfo)
    {
        _nonSecuredObjectSpaceFactory = nonSecuredObjectSpaceFactory;
        _typesInfo = typesInfo;
    }

    /// <summary>
    /// Executes a global full-text search across all persistent types.
    /// </summary>
    /// <param name="keyword">Search keyword (required)</param>
    /// <param name="maxResults">Maximum total results (optional, default: 200)</param>
    /// <param name="maxPerType">Maximum results per type (optional, default: 50)</param>
    /// <returns>Search results with matched content</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GlobalSearchResponseDto), 200)]
    [ProducesResponseType(400)]
    public IActionResult Search(
        [FromQuery] string keyword,
        [FromQuery] int? maxResults = 200,
        [FromQuery] int? maxPerType = 50)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest(new { error = "Keyword is required." });

        using var objectSpace = _nonSecuredObjectSpaceFactory.CreateNonSecuredObjectSpace(typeof(object));

        var searchService = new GlobalSearchService(objectSpace, _typesInfo)
        {
            MaxTotalResults = maxResults ?? 200,
            MaxResultsPerType = maxPerType ?? 50
        };

        var results = searchService.Search(keyword);

        var response = new GlobalSearchResponseDto
        {
            Keyword = keyword,
            TotalCount = results.Count,
            Results = results.Select(r => new GlobalSearchResultDto
            {
                DisplayName = r.DisplayName,
                ObjectKey = r.TargetObjectKey,
                TypeCaption = r.TypeCaption,
                TypeFullName = r.TargetObjectType?.FullName,
                MatchedContent = r.MatchedContent
            }).ToList()
        };

        return Ok(response);
    }
}
