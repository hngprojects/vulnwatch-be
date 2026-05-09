

using Domain.Common;

namespace Application.Interfaces;

public record AuthTokens(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);

public interface ISessionService
{
    Task<Result<AuthTokens>> IssueTokens(Guid userId, string email,
        string? ipAddress = null, CancellationToken ct = default);

    Task<Result<AuthTokens>> Refresh(string refreshToken, string? ipAddress = null,
        CancellationToken ct = default);
 
    Task<Result<bool>> Revoke(string refreshToken, CancellationToken ct = default);

    Task<Result<bool>> RevokeAll(Guid userId, CancellationToken ct = default);
}
