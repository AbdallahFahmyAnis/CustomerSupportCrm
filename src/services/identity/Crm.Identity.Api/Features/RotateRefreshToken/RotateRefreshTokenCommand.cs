using Crm.Identity.Api.Features.IssueToken;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.RotateRefreshToken;

/// <summary>SDD CRM-035 — rotate refresh token and issue new access token.</summary>
public sealed record RotateRefreshTokenCommand(string RefreshToken) : IRequest<IssueTokenResult>;

public sealed class RotateRefreshTokenHandler(IdentityDb db, TokenService tokens)
    : IRequestHandler<RotateRefreshTokenCommand, IssueTokenResult>
{
    public Task<IssueTokenResult> Handle(RotateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Task.FromResult(new IssueTokenResult(null, "Refresh token is required.", false));
        }

        var now = DateTimeOffset.UtcNow;
        var hash = TokenService.HashToken(request.RefreshToken);
        var existing = db.FindRefreshTokenByHash(hash);
        if (existing is null || !existing.IsActive(now))
        {
            if (existing is not null)
            {
                // reuse of revoked/expired refresh → revoke all for user (theft detection)
                db.RevokeAllRefreshTokensForUser(existing.UserId, now);
            }

            return Task.FromResult(new IssueTokenResult(null, "Invalid or revoked refresh token.", false));
        }

        var user = db.GetUser(existing.UserId);
        if (user is null || !user.IsActive || user.IsLockedOut(now))
        {
            db.RevokeRefreshToken(existing.Id, now);
            return Task.FromResult(new IssueTokenResult(null, "User cannot refresh token.", false));
        }

        var pair = IssueTokenHandler.IssuePair(db, tokens, user, now);
        var newHash = TokenService.HashToken(pair.RefreshToken);
        var replacement = db.FindRefreshTokenByHash(newHash);
        db.RevokeRefreshToken(existing.Id, now, replacement?.Id);
        return Task.FromResult(new IssueTokenResult(pair, null, false));
    }
}
