using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.UpdatePermission;

public sealed class UpdatePermissionHandler(IdentityDirectory directory)
    : IRequestHandler<UpdatePermissionCommand, UpdatePermissionResponse>
{
    public async Task<UpdatePermissionResponse> Handle(
        UpdatePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var (row, error) = await directory.UpdatePermissionAsync(
            request.CurrentName,
            request.Name,
            request.Description,
            cancellationToken);
        if (error is not null || row is null)
        {
            return new UpdatePermissionResponse(null, error ?? "Update failed.");
        }

        string? actorEmail = null;
        if (request.ActorId is { } actorId)
        {
            actorEmail = (await directory.GetUserAsync(actorId, cancellationToken))?.Email;
        }

        await directory.AppendAuditAsync(
            AuditActions.PermissionUpdated,
            true,
            request.ActorId,
            actorEmail,
            null,
            null,
            $"{request.CurrentName} → {row.Name}",
            cancellationToken);

        return new UpdatePermissionResponse(row.Name, null);
    }
}
