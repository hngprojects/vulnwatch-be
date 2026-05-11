using Application.Features.Scans;
using Application.Features.Scans.GetScanHistory;
using Application.Interfaces;
using Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

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
}
