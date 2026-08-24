using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Crm.Identity.Api.Infrastructure;

/// <summary>SDD CRM-035 — JWT access token issuer/validator.</summary>
public sealed class TokenService
{
    private readonly byte[] _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly TimeSpan _accessLifetime;
    private readonly TimeSpan _refreshLifetime;

    public TokenService(IConfiguration config)
    {
        var signing = config["Identity:Jwt:SigningKey"]
                      ?? "CrmLocalDevSigningKey-ChangeMe-AtLeast32Chars!";
        _key = Encoding.UTF8.GetBytes(signing);
        _issuer = config["Identity:Jwt:Issuer"] ?? "crm.identity";
        _audience = config["Identity:Jwt:Audience"] ?? "crm.gateway";
        _accessLifetime = TimeSpan.FromMinutes(
            int.TryParse(config["Identity:Jwt:AccessTokenMinutes"], out var am) ? am : 15);
        _refreshLifetime = TimeSpan.FromDays(
            int.TryParse(config["Identity:Jwt:RefreshTokenDays"], out var rd) ? rd : 7);
    }

    public TimeSpan AccessLifetime => _accessLifetime;
    public TimeSpan RefreshLifetime => _refreshLifetime;

    public (string Token, string Jti, DateTimeOffset ExpiresAt) CreateAccessToken(UserClaims user)
    {
        var jti = Guid.NewGuid().ToString("N");
        var expires = DateTimeOffset.UtcNow.Add(_accessLifetime);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(_key),
            SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires.UtcDateTime,
            signingCredentials: credentials);
        return (new JwtSecurityTokenHandler().WriteToken(jwt), jti, expires);
    }

    public static string CreateRefreshTokenValue()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }

    public ClaimsPrincipal? ValidateAccessToken(string token, bool validateLifetime = true)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(_key),
            ValidateLifetime = validateLifetime,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        try
        {
            return new JwtSecurityTokenHandler().ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }

    public sealed record UserClaims(Guid Id, string Email, string DisplayName, string Role);
}
