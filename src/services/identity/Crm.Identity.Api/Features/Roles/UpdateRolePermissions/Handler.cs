using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.UpdateRolePermissions;

public sealed class UpdateRolePermissionsHandler(IdentityDirectory directory)
    : IRequestHandler<UpdateRolePermissionsCommand, UpdateRolePermissionsResponse>
{
    public async Task<UpdateRolePermissionsResponse> Handle(
        UpdateRolePermissionsCommand request,
        CancellationToken cancellationToken)
    {
        var (role, error) = await directory.SetRolePermissionsAsync(
            request.RoleName,
            request.Permissions,
            cancellationToken);
        if (error is not null || role is null)
        {
            return new UpdateRolePermissionsResponse(null, error ?? "Update failed.");
        }

        string? actorEmail = null;
        if (request.ActorId is { } actorId)
        {
            actorEmail = (await directory.GetUserAsync(actorId, cancellationToken))?.Email;
        }

        await directory.AppendAuditAsync(
            AuditActions.RolePermissionsUpdated,
            true,
            request.ActorId,
            actorEmail,
            null,
            null,
            $"{role.Name}: {string.Join(", ", role.Permissions)}",
            cancellationToken);

        return new UpdateRolePermissionsResponse(
            new RoleSummaryDto(role.Name, role.Description, role.Permissions),
            null);
    }
}
