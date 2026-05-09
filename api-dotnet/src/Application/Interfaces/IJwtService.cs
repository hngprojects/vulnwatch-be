using Domain.Entities;

namespace Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
 
    Result<TokenClaims> ValidateAccessToken(string token);

    string GenerateRefreshTokenString();
}
