using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.IssueToken;

/// <summary>SDD CRM-035 — password login → access + refresh tokens.</summary>
public sealed record IssueTokenCommand(string Email, string Password) : IRequest<IssueTokenResult>;

public sealed record IssueTokenResult(TokenResponseDto? Tokens, string? Error, bool Locked);

public sealed class IssueTokenHandler(IdentityDb db, TokenService tokens)
    : IRequestHandler<IssueTokenCommand, IssueTokenResult>
{
    public Task<IssueTokenResult> Handle(IssueTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult(new IssueTokenResult(null, "Email and password are required.", false));
        }

        var now = DateTimeOffset.UtcNow;
        var user = db.FindByEmail(request.Email);
        if (user is null)
        {
            return Task.FromResult(new IssueTokenResult(null, "Invalid credentials.", false));
        }

        if (!user.IsActive)
        {
            return Task.FromResult(new IssueTokenResult(null, "Account is inactive.", false));
        }

        if (user.IsLockedOut(now))
        {
            return Task.FromResult(new IssueTokenResult(
                null,
                $"Account locked until {user.LockoutUntil:u}.",
                true));
        }

        if (!user.VerifyPassword(request.Password))
        {
            user.RegisterFailedLogin(now);
            db.Update(user);
            var locked = user.IsLockedOut(now);
            return Task.FromResult(new IssueTokenResult(
                null,
                locked ? $"Account locked until {user.LockoutUntil:u}." : "Invalid credentials.",
                locked));
        }

        user.RegisterSuccessfulLogin();
        db.Update(user);
        return Task.FromResult(new IssueTokenResult(IssuePair(db, tokens, user, now), null, false));
    }

    internal static TokenResponseDto IssuePair(IdentityDb db, TokenService tokens, UserAccount user, DateTimeOffset now)
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
