using Application.Features.Scans;
using Application.Features.Scans.GetScanHistory;
using Application.Features.Scans.GetScanDetail;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;
using System.Security.Claims;

namespace Web.Controllers;

/**
 * ScansController: Handles all HTTP requests related to vulnerability scans.
 * Intern-friendly: This is the entry point for the API.
 */
[ApiController]
[Route("api/[controller]")]
public class ScansController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUser _currentUser;

    public ScansController(IMediator mediator, ICurrentUser currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<ActionResult<Result<ScanResponse>>> Create(
        [FromHeader(Name = "Idempotency-Key")] Guid idempotencyKey,
        [FromBody] CreateScanRequest body)
    {
        var command = new CreateScanCommand(idempotencyKey, body.Domain, body.ScanTypes);
        var result = await _mediator.Send(command);
        return result.ToHttpResponse(this);
    }

    [HttpGet("domains/{domainId:guid}/scans")]
    public async Task<ActionResult<Result<ScanHistoryResponse>>> GetHistory(
        Guid domainId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetScanHistoryQuery(domainId, _currentUser.UserId, page, pageSize);
        var result = await _mediator.Send(query);
        return result.ToHttpResponse(this);
    }

    /// <summary>
    /// Get full scan detail including all findings.
    /// Poll this after triggering a scan to track status.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<Result<ScanDetailResponse>>> GetDetail(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var query = new GetScanDetailQuery(id, userId);
        var result = await _mediator.Send(query);

        // Return 202 Accepted if scan is still queued or running (frontend should keep polling)
        if (result.IsSuccess && (result.Value!.Status == "Queued" || result.Value!.Status == "Running"))
            return Accepted(result);

        return result.ToHttpResponse(this);
    }
}
