using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.RevokeToken;

/// <summary>SDD CRM-035 — revoke refresh and/or access (jti blacklist).</summary>
public sealed record RevokeTokenCommand(string? RefreshToken, string? AccessToken) : IRequest<RevokeTokenResult>;

public sealed record RevokeTokenResult(bool Ok, string? Error);

public sealed class RevokeTokenHandler(IdentityDb db, TokenService tokens)
    : IRequestHandler<RevokeTokenCommand, RevokeTokenResult>
{
    public Task<RevokeTokenResult> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var any = false;

        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var existing = db.FindRefreshTokenByHash(TokenService.HashToken(request.RefreshToken));
            if (existing is not null && existing.RevokedAt is null)
            {
                db.RevokeRefreshToken(existing.Id, now);
                any = true;
            }
        }

        if (!string.IsNullOrWhiteSpace(request.AccessToken))
        {
            var principal = tokens.ValidateAccessToken(request.AccessToken, validateLifetime: false);
            if (principal is not null)
            {
                var jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
                var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var expClaim = principal.FindFirstValue(JwtRegisteredClaimNames.Exp);
                if (!string.IsNullOrWhiteSpace(jti) && Guid.TryParse(sub, out var userId))
                {
                    var exp = DateTimeOffset.UtcNow.AddMinutes(15);
                    if (long.TryParse(expClaim, out var unix))
                    {
                        exp = DateTimeOffset.FromUnixTimeSeconds(unix);
                    }

                    db.RevokeAccessJti(jti, userId, exp, now);
                    any = true;
                }
            }
        }

        return Task.FromResult(any
            ? new RevokeTokenResult(true, null)
            : new RevokeTokenResult(false, "No active token to revoke."));
    }
}
