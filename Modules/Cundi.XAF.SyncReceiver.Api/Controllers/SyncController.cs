using Cundi.XAF.SyncReceiver.DTOs;
using Cundi.XAF.SyncReceiver.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cundi.XAF.SyncReceiver.Api.Controllers;

/// <summary>
/// API controller for receiving sync webhook requests.
/// Processes incoming sync payloads and applies changes to the local database.
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize] // Use API Key or JWT authentication
public class SyncController : ControllerBase
{
    private readonly SyncService _syncService;

    public SyncController(SyncService syncService)
    {
        _syncService = syncService;
    }

    /// <summary>
    /// Receives a single sync webhook request.
    /// </summary>
    /// <param name="payload">The sync payload containing event type, object type, and data.</param>
    /// <returns>The result of the sync operation.</returns>
    [HttpPost]
    public IActionResult Sync([FromBody] SyncPayloadDto payload)
    {
        if (payload == null)
        {
            return BadRequest(new { success = false, message = "Payload is required." });
        }

        var result = _syncService.ProcessSync(payload);

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
    /// Receives a batch of sync webhook requests.
    /// Processes multiple sync payloads in sequence.
    /// </summary>
    /// <param name="payloads">List of sync payloads to process.</param>
    /// <returns>Results for each sync operation.</returns>
    [HttpPost("batch")]
    public IActionResult SyncBatch([FromBody] List<SyncPayloadDto> payloads)
    {
        if (payloads == null || payloads.Count == 0)
        {
            return BadRequest(new { success = false, message = "Payloads are required." });
        }

        var results = new List<object>();
        var hasErrors = false;

        foreach (var payload in payloads)
        {
            var result = _syncService.ProcessSync(payload);
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
    /// Health check endpoint to verify the sync API is operational.
    /// </summary>
    /// <returns>OK status if the API is healthy.</returns>
    [HttpGet("health")]
    [AllowAnonymous]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow.ToString("o") });
    }
}
