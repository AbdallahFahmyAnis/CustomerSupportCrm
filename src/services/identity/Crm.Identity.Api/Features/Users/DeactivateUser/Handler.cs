using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Users.DeactivateUser;

public sealed class DeactivateUserHandler(IdentityDb db) : IRequestHandler<DeactivateUserCommand, DeactivateUserResponse>
{
    public Task<DeactivateUserResponse> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = db.GetUser(request.UserId);
        if (user is null)
        {
            return Task.FromResult(new DeactivateUserResponse(null, "User not found."));
        }

        if (request.ActorId.HasValue && request.ActorId.Value == user.Id)
        {
            return Task.FromResult(new DeactivateUserResponse(null, "You cannot deactivate your own account."));
        }

        user.Deactivate();
        db.Update(user);
        return Task.FromResult(new DeactivateUserResponse(
            new UserSummaryDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role, user.IsActive),
            null));
    }
}
