using System.Security.Claims;
using System.Text;
using Application.Helpers;
using Application.Interfaces;
using Domain.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly JwtConfig _options;
    private readonly SymmetricSecurityKey _signingKey;
 
    public JwtService(IOptions<JwtConfig> options)
    {
        _options = options.Value;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
    }
 
    public string GenerateAccessToken(Guid userId, string email)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };
 
        var token = new JwtSecurityToken(
            issuer:             _options.Issuer,
            audience:           _options.Audience,
            claims:             claims,
            notBefore:          DateTime.UtcNow,
            expires:            DateTime.UtcNow.AddMinutes(_options.AccessTokenExpiryMinutes),
            signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
        );
 
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
 
    public string GenerateRefreshTokenString()
    {
        var bytes = new byte[32];
        System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    public Result<TokenClaims> ValidateAccessToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
 
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidIssuer              = _options.Issuer,
            ValidateAudience         = true,
            ValidAudience            = _options.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = _signingKey,
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.FromSeconds(30) // Small tolerance for clock drift
        };
 
        try
        {
            var principal = handler.ValidateToken(token, validationParams, out _);
 
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var email  = principal.FindFirstValue(ClaimTypes.Email)
                         ?? principal.FindFirstValue(JwtRegisteredClaimNames.Email);
            var role   = principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
 
            if (userId is null || email is null)
                return Result<TokenClaims>.Failure(Error.Unauthorized("Token is invalid"));
 
            return Result<TokenClaims>.Success(new TokenClaims(Guid.Parse(userId), email));
        }
        catch (SecurityTokenExpiredException)
        {
            return Result<TokenClaims>.Failure(Error.Unauthorized("Token is expired"));
        }
        catch (SecurityTokenException)
        {
            return Result<TokenClaims>.Failure(Error.Unauthorized("Token is invalid"));
        }
    }
}