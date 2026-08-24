using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.DeactivateUser;

/// <summary>SDD CRM-035.</summary>
public sealed record DeactivateUserCommand(Guid UserId, Guid? ActorId) : IRequest<DeactivateUserResult>;

public sealed record DeactivateUserResult(UserSummaryDto? User, string? Error);

public sealed class DeactivateUserHandler(IdentityDb db) : IRequestHandler<DeactivateUserCommand, DeactivateUserResult>
{
    public Task<DeactivateUserResult> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = db.GetUser(request.UserId);
        if (user is null)
        {
            return Task.FromResult(new DeactivateUserResult(null, "User not found."));
        }

        if (request.ActorId.HasValue && request.ActorId.Value == user.Id)
        {
            return Task.FromResult(new DeactivateUserResult(null, "You cannot deactivate your own account."));
        }

        user.Deactivate();
        db.Update(user);
        return Task.FromResult(new DeactivateUserResult(
            new UserSummaryDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role, user.IsActive),
            null));
    }
}
