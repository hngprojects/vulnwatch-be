using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Scans;
using Web.Extensions;
using Application.Features.Authentication;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest body)
    {
        var command = new ForgotPasswordCommand(body.Email);
        var result = await _mediator.Send(command);
        return result.ToHttpResponse(this);
    }
}