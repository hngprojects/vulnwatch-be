
using Domain.Common;
using MediatR;
using System.ComponentModel.DataAnnotations;


namespace Application.Features.Authentication;

public record ForgotPasswordRequest
{
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    public string Email { get; init; } = string.Empty;
}


public record ForgotPasswordCommand(string Email) : IRequest<Result<bool>>;

