using Domain.Common;

namespace Application.Interfaces;
public record TokenClaims(Guid UserId, string Email);
public interface IJwtService
{
    string GenerateAccessToken(Guid userId, string email);
 
    Result<TokenClaims> ValidateAccessToken(string token);

    string GenerateRefreshTokenString();
}