using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Users.DeactivateUser;

public sealed class DeactivateUserHandler(IdentityDirectory directory)
    : IRequestHandler<DeactivateUserCommand, DeactivateUserResponse>
{
    public async Task<DeactivateUserResponse> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await directory.GetUserAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return new DeactivateUserResponse(null, "User not found.");
        }

        if (request.ActorId.HasValue && request.ActorId.Value == user.Id)
        {
            return new DeactivateUserResponse(null, "You cannot deactivate your own account.");
        }

        user.Deactivate();
        await directory.UpdateAsync(user, cancellationToken);

        string? actorEmail = null;
        if (request.ActorId is { } actorId)
        {
            var actor = await directory.GetUserAsync(actorId, cancellationToken);
            actorEmail = actor?.Email;
        }

        await directory.AppendAuditAsync(
            AuditActions.UserDeactivated,
            true,
            request.ActorId,
            actorEmail,
            user.Id,
            user.Email,
            null,
            cancellationToken);

        return new DeactivateUserResponse(
            new UserSummaryDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role, user.IsActive),
            null);
    }
}
