using System.Net;
using Application.Features.Auth.DTOs;
using Application.Features.Domain.DTOs;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Features.Domain;

public record RegisterDomainCommand(string Domain) : IRequest<Result<RegisterDomainResponse>>;

public class RegisterDomainHandler : IRequestHandler<RegisterDomainCommand, Result<RegisterDomainResponse>>
{
    private readonly IScannedDomainRepository _domains;
    private readonly ICurrentUser _currentUser;
    private readonly ITokenService _token;
    private readonly ILogger<RegisterDomainHandler> _logger;

    public RegisterDomainHandler(IScannedDomainRepository domains, ICurrentUser currentUser, ITokenService token, ILogger<RegisterDomainHandler> logger)
    {
        _domains = domains;
        _currentUser = currentUser;
        _token = token;
        _logger = logger;
    }

    public async Task<Result<RegisterDomainResponse>> Handle(RegisterDomainCommand cmd, CancellationToken ct)
    {
        var domain = cmd.Domain.ToLowerInvariant();

        var existing = await _domains.FindActive(domain, ct);

        if (existing is not null)
            return Result<RegisterDomainResponse>.Failure(Error.Conflict($"Domain '{domain}' is already registered."));

        var unverifiedCount = await _domains.CountPending(_currentUser.UserId, ct);
        if (unverifiedCount >= 20)
            return Result<RegisterDomainResponse>.Failure(Error.RateLimited("You have reached the maximum of 20 pending domains. Verify or remove existing ones before adding more."));

        var (rawToken, tokenHash) = _token.Generate();

        var record = ScannedDomain.Create(
            _currentUser.UserId,
            domain,
            tokenHash);

        await _domains.AddAsync(record, ct);
        await _domains.SaveChangesAsync(ct);
        return Result<RegisterDomainResponse>.Success(RegisterDomainResponse.Create(rawToken, record));
    }
}
