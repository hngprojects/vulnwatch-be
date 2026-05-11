using Domain.Common;

namespace Application.Interfaces;

public interface IGoogleTokenVerifier
{
    Task<Result<GoogleUserInfo>> VerifyIdTokenAsync(string IdToken, CancellationToken ct);
}

public sealed record GoogleUserInfo(string Subject, string Email, bool EmailVerified);
