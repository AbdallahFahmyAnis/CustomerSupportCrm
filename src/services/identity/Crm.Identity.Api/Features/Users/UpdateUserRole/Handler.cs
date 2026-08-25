using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Users.UpdateUserRole;

public sealed class UpdateUserRoleHandler(IdentityDirectory directory)
    : IRequestHandler<UpdateUserRoleCommand, UpdateUserRoleResponse>
{
    public async Task<UpdateUserRoleResponse> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var validation = UpdateUserRoleValidator.Validate(request);
        if (validation is not null)
        {
            return new UpdateUserRoleResponse(null, validation);
        }

        try
        {
            var user = await directory.GetUserAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                return new UpdateUserRoleResponse(null, "User not found.");
            }

            var previous = user.Role;
            user.AssignRole(request.Role);
            await directory.UpdateAsync(user, cancellationToken);

            string? actorEmail = null;
            if (request.ActorId is { } actorId)
            {
                var actor = await directory.GetUserAsync(actorId, cancellationToken);
                actorEmail = actor?.Email;
            }

            await directory.AppendAuditAsync(
                AuditActions.RoleChanged,
                true,
                request.ActorId,
                actorEmail,
                user.Id,
                user.Email,
                $"{previous} → {user.Role}",
                cancellationToken);

            return new UpdateUserRoleResponse(
                new UserSummaryDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role, user.IsActive),
                null);
        }
        catch (Exception ex)
        {
            return new UpdateUserRoleResponse(null, ex.Message);
        }
    }
}
