using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Auth;

public record ForgotPasswordCommand(string Email) : IRequest<Result<MessageResponse>>;

public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result<MessageResponse>>
{
    private readonly UserManager<User> _userManager;
    private readonly IEmailService _email;
    private readonly IConfiguration _config;

    public ForgotPasswordHandler(UserManager<User> userManager, IEmailService email, IConfiguration config)
    {
        _userManager = userManager;
        _email = email;
        _config = config;
    }

    public async Task<Result<MessageResponse>> Handle(ForgotPasswordCommand cmd, CancellationToken ct)
    {
        // Always return the same message so we don't reveal whether the email exists
        const string message = "If this email is registered, a reset link has been sent.";

        var user = await _userManager.FindByEmailAsync(cmd.Email);
        if (user is null)
            return Result<MessageResponse>.Success(MessageResponse.Create(message));

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var frontendUrl = _config["FrontendUrl:path"];
        var resetLink = $"{frontendUrl}/reset-password?email={Uri.EscapeDataString(cmd.Email)}&token={Uri.EscapeDataString(token)}";

        await _email.SendAsync(cmd.Email, "Reset your password", $"Click here to reset your password: {resetLink}");

        return Result<MessageResponse>.Success(MessageResponse.Create(message));
    }
}
