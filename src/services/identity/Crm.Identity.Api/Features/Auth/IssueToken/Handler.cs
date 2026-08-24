using Crm.Identity.Api.Features.Auth.IssueToken;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.IssueToken;

public sealed class IssueTokenHandler(IdentityDirectory directory)
    : IRequestHandler<IssueTokenCommand, IssueTokenResponse>
{
    public async Task<IssueTokenResponse> Handle(IssueTokenCommand request, CancellationToken cancellationToken)
    {
        var validation = IssueTokenValidator.Validate(request);
        if (validation is not null)
        {
            return new IssueTokenResponse(null, validation, false);
        }

        var now = DateTimeOffset.UtcNow;
        var user = await directory.FindByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return new IssueTokenResponse(null, "Invalid credentials.", false);
        }

        if (!user.IsActive)
        {
            return new IssueTokenResponse(null, "Account is inactive.", false);
        }

        if (user.IsLockedOut(now))
        {
            return new IssueTokenResponse(
                null,
                $"Account locked until {user.LockoutUntil:u}.",
                true);
        }

        if (!await directory.CheckPasswordAsync(user, request.Password))
        {
            await directory.RegisterFailedLoginAsync(user, now, cancellationToken);
            var refreshed = await directory.GetUserAsync(user.Id, cancellationToken);
            var locked = refreshed is not null && await directory.IsLockedOutAsync(user.Id, cancellationToken);
            return new IssueTokenResponse(
                null,
                locked ? $"Account locked until {refreshed!.LockoutUntil:u}." : "Invalid credentials.",
                locked);
        }

        await directory.RegisterSuccessfulLoginAsync(user, cancellationToken);
        var pair = await directory.IssuePairAsync(user, now, cancellationToken);
        return new IssueTokenResponse(pair, null, false);
    }
}
