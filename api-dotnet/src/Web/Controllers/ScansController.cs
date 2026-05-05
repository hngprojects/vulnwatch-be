using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.Features.Scans;

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

    public ScansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateScanCommand command)
    {
        // We use MediatR to keep controllers thin. 
        // The controller just passes the command to the right Handler.
        var scanId = await _mediator.Send(command);
        return Accepted(new { ScanId = scanId, Message = "Scan requested successfully" });
    }
}
