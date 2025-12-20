using Cundi.XAF.DataMirror.DTOs;
using Cundi.XAF.DataMirror.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cundi.XAF.DataMirror.Api.Controllers;

/// <summary>
/// API controller for receiving mirror webhook requests.
/// Processes incoming mirror payloads and applies changes to the local database.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize] // Use API Key or JWT authentication
public class MirrorController : ControllerBase
{
    private readonly MirrorService _mirrorService;

    public MirrorController(MirrorService mirrorService)
    {
        _mirrorService = mirrorService;
    }

    /// <summary>
    /// Receives a single mirror webhook request.
    /// </summary>
    /// <param name="payload">The mirror payload containing event type, object type, and data.</param>
    /// <returns>The result of the mirror operation.</returns>
    [HttpPost]
    public IActionResult Mirror([FromBody] MirrorPayloadDto payload)
    {
        if (payload == null)
        {
            return BadRequest(new { success = false, message = "Payload is required." });
        }

        var result = _mirrorService.ProcessMirror(payload);

        if (result.IsSuccess)
        {
            return Ok(new { success = true, message = result.Message });
        }
        else
        {
            return BadRequest(new { success = false, message = result.Message });
        }
    }

    /// <summary>
    /// Receives a batch of mirror webhook requests.
    /// Processes multiple mirror payloads in sequence.
    /// </summary>
    /// <param name="payloads">List of mirror payloads to process.</param>
    /// <returns>Results for each mirror operation.</returns>
    [HttpPost("batch")]
    public IActionResult MirrorBatch([FromBody] List<MirrorPayloadDto> payloads)
    {
        if (payloads == null || payloads.Count == 0)
        {
            return BadRequest(new { success = false, message = "Payloads are required." });
        }

        var results = new List<object>();
        var hasErrors = false;

        foreach (var payload in payloads)
        {
            var result = _mirrorService.ProcessMirror(payload);
            results.Add(new
            {
                objectKey = payload.ObjectKey,
                objectType = payload.ObjectType,
                eventType = payload.EventType,
                success = result.IsSuccess,
                message = result.Message
            });
            if (!result.IsSuccess) hasErrors = true;
        }

        return Ok(new { success = !hasErrors, results });
    }

    /// <summary>
    /// Health check endpoint to verify the mirror API is operational.
    /// </summary>
    /// <returns>OK status if the API is healthy.</returns>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow.ToString("o") });
    }
}
