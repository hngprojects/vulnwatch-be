using Application.Features.Auth.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth;

public record RegisterCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;

public class RegisterHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwt;

    public RegisterHandler(UserManager<User> userManager, IJwtService jwt)
    {
        _userManager = userManager;
        _jwt = jwt;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        var existing = await _userManager.FindByEmailAsync(cmd.Email);
        if (existing is not null)
            return Result<AuthResponse>.Failure(Error.Conflict("Email is already registered."));

        var user = User.Create(cmd.Email);
        var result = await _userManager.CreateAsync(user, cmd.Password);

        if (!result.Succeeded)
            return Result<AuthResponse>.Failure(Error.Validation(result.Errors.First().Description));

        var token = _jwt.GenerateToken(user);
        return Result<AuthResponse>.Success(AuthResponse.Create(token, user));
    }
}
