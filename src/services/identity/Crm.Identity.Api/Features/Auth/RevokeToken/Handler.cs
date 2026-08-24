using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.RevokeToken;

public sealed class RevokeTokenHandler(IdentityDirectory directory, TokenService tokens)
    : IRequestHandler<RevokeTokenCommand, RevokeTokenResponse>
{
    public async Task<RevokeTokenResponse> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var any = false;

        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var existing = await directory.FindRefreshTokenByHashAsync(
                TokenService.HashToken(request.RefreshToken), cancellationToken);
            if (existing is not null && existing.RevokedAt is null)
            {
                await directory.RevokeRefreshTokenAsync(existing.Id, now, null, cancellationToken);
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

                    await directory.RevokeAccessJtiAsync(jti, userId, exp, now, cancellationToken);
                    any = true;
                }
            }
        }

        return any
            ? new RevokeTokenResponse(true, null)
            : new RevokeTokenResponse(false, "No active token to revoke.");
    }
}
