using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth;

public record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;

public class LoginHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwt;

    public LoginHandler(UserManager<User> userManager, IJwtService jwt)
    {
        _userManager = userManager;
        _jwt = jwt;
    }

    public async Task<Result<AuthResponse>> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(cmd.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, cmd.Password))
            return Result<AuthResponse>.Failure(Error.Unauthorized("Invalid email or password."));

        var token = _jwt.GenerateToken(user);
        return Result<AuthResponse>.Success(AuthResponse.Create(token, user));
    }
}
