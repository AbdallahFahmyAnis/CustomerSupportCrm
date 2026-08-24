using Crm.Identity.Api.Features.Auth.IssueToken;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.RotateRefreshToken;

public sealed class RotateRefreshTokenHandler(IdentityDirectory directory)
    : IRequestHandler<RotateRefreshTokenCommand, IssueTokenResponse>
{
    public async Task<IssueTokenResponse> Handle(RotateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var validation = RotateRefreshTokenValidator.Validate(request);
        if (validation is not null)
        {
            return new IssueTokenResponse(null, validation, false);
        }

        var now = DateTimeOffset.UtcNow;
        var hash = TokenService.HashToken(request.RefreshToken);
        var existing = await directory.FindRefreshTokenByHashAsync(hash, cancellationToken);
        if (existing is null || !existing.IsActive(now))
        {
            if (existing is not null)
            {
                await directory.RevokeAllRefreshTokensForUserAsync(existing.UserId, now, cancellationToken);
            }

            return new IssueTokenResponse(null, "Invalid or revoked refresh token.", false);
        }

        var user = await directory.GetUserAsync(existing.UserId, cancellationToken);
        if (user is null || !user.IsActive || user.IsLockedOut(now))
        {
            await directory.RevokeRefreshTokenAsync(existing.Id, now, null, cancellationToken);
            return new IssueTokenResponse(null, "User cannot refresh token.", false);
        }

        var pair = await directory.IssuePairAsync(user, now, cancellationToken);
        var replacement = await directory.FindRefreshTokenByHashAsync(
            TokenService.HashToken(pair.RefreshToken), cancellationToken);
        await directory.RevokeRefreshTokenAsync(existing.Id, now, replacement?.Id, cancellationToken);
        return new IssueTokenResponse(pair, null, false);
    }
}
