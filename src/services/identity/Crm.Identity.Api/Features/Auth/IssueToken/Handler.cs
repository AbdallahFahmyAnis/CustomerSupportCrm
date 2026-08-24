using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.IssueToken;

public sealed class IssueTokenHandler(IdentityDb db, TokenService tokens)
    : IRequestHandler<IssueTokenCommand, IssueTokenResponse>
{
    public Task<IssueTokenResponse> Handle(IssueTokenCommand request, CancellationToken cancellationToken)
    {
        var validation = IssueTokenValidator.Validate(request);
        if (validation is not null)
        {
            return Task.FromResult(new IssueTokenResponse(null, validation, false));
        }

        var now = DateTimeOffset.UtcNow;
        var user = db.FindByEmail(request.Email);
        if (user is null)
        {
            return Task.FromResult(new IssueTokenResponse(null, "Invalid credentials.", false));
        }

        if (!user.IsActive)
        {
            return Task.FromResult(new IssueTokenResponse(null, "Account is inactive.", false));
        }

        if (user.IsLockedOut(now))
        {
            return Task.FromResult(new IssueTokenResponse(
                null,
                $"Account locked until {user.LockoutUntil:u}.",
                true));
        }

        if (!user.VerifyPassword(request.Password))
        {
            user.RegisterFailedLogin(now);
            db.Update(user);
            var locked = user.IsLockedOut(now);
            return Task.FromResult(new IssueTokenResponse(
                null,
                locked ? $"Account locked until {user.LockoutUntil:u}." : "Invalid credentials.",
                locked));
        }

        user.RegisterSuccessfulLogin();
        db.Update(user);
        return Task.FromResult(new IssueTokenResponse(TokenIssuer.IssuePair(db, tokens, user, now), null, false));
    }
}
