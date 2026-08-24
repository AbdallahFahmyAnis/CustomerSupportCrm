using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;

namespace Crm.Identity.Api.Infrastructure;

/// <summary>Shared access/refresh pair issuance used by Auth slices.</summary>
public static class TokenIssuer
{
    public static TokenResponseDto IssuePair(IdentityDb db, TokenService tokens, UserAccount user, DateTimeOffset now)
    {
        var (access, _, accessExp) = tokens.CreateAccessToken(
            new TokenService.UserClaims(user.Id, user.Email, user.DisplayName, user.Role));
        var refreshValue = TokenService.CreateRefreshTokenValue();
        var refresh = new StoredRefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = TokenService.HashToken(refreshValue),
            ExpiresAt = now.Add(tokens.RefreshLifetime),
            CreatedAt = now
        };
        db.InsertRefreshToken(refresh);

        return new TokenResponseDto(
            access,
            refreshValue,
            accessExp,
            refresh.ExpiresAt,
            new DevUserDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role));
    }
}
