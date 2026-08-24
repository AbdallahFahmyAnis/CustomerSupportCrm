using Crm.Identity.Api.Features.Auth.IssueToken;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.RotateRefreshToken;

public sealed class RotateRefreshTokenHandler(IdentityDb db, TokenService tokens)
    : IRequestHandler<RotateRefreshTokenCommand, IssueTokenResponse>
{
    public Task<IssueTokenResponse> Handle(RotateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var validation = RotateRefreshTokenValidator.Validate(request);
        if (validation is not null)
        {
            return Task.FromResult(new IssueTokenResponse(null, validation, false));
        }

        var now = DateTimeOffset.UtcNow;
        var hash = TokenService.HashToken(request.RefreshToken);
        var existing = db.FindRefreshTokenByHash(hash);
        if (existing is null || !existing.IsActive(now))
        {
            if (existing is not null)
            {
                db.RevokeAllRefreshTokensForUser(existing.UserId, now);
            }

            return Task.FromResult(new IssueTokenResponse(null, "Invalid or revoked refresh token.", false));
        }

        var user = db.GetUser(existing.UserId);
        if (user is null || !user.IsActive || user.IsLockedOut(now))
        {
            db.RevokeRefreshToken(existing.Id, now);
            return Task.FromResult(new IssueTokenResponse(null, "User cannot refresh token.", false));
        }

        var pair = TokenIssuer.IssuePair(db, tokens, user, now);
        var replacement = db.FindRefreshTokenByHash(TokenService.HashToken(pair.RefreshToken));
        db.RevokeRefreshToken(existing.Id, now, replacement?.Id);
        return Task.FromResult(new IssueTokenResponse(pair, null, false));
    }
}
