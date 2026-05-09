namespace Application.Features.Scans;

using MediatR;
using Application.Interfaces;
using Domain.Common;

public record ForgotPasswordCommand(string Email) : IRequest<Result<bool>>;

// public record Result(Guid ScanId, string Status);
