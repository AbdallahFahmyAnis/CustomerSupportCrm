using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.DeletePermission;

public sealed class DeletePermissionHandler(IdentityDirectory directory)
    : IRequestHandler<DeletePermissionCommand, DeletePermissionResponse>
{
    public async Task<DeletePermissionResponse> Handle(
        DeletePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var error = await directory.DeletePermissionAsync(request.Name, cancellationToken);
        if (error is not null)
        {
            return new DeletePermissionResponse(error);
        }

        string? actorEmail = null;
        if (request.ActorId is { } actorId)
        {
            actorEmail = (await directory.GetUserAsync(actorId, cancellationToken))?.Email;
        }

        await directory.AppendAuditAsync(
            AuditActions.PermissionDeleted,
            true,
            request.ActorId,
            actorEmail,
            null,
            null,
            request.Name.Trim().ToLowerInvariant(),
            cancellationToken);

        return new DeletePermissionResponse(null);
    }
}
