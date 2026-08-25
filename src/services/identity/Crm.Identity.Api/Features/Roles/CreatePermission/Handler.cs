using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.CreatePermission;

public sealed class CreatePermissionHandler(IdentityDirectory directory)
    : IRequestHandler<CreatePermissionCommand, CreatePermissionResponse>
{
    public async Task<CreatePermissionResponse> Handle(
        CreatePermissionCommand request,
        CancellationToken cancellationToken)
    {
        var (row, error) = await directory.CreatePermissionAsync(
            request.Name,
            request.Description,
            cancellationToken);
        if (error is not null || row is null)
        {
            return new CreatePermissionResponse(null, error ?? "Create failed.");
        }

        string? actorEmail = null;
        if (request.ActorId is { } actorId)
        {
            actorEmail = (await directory.GetUserAsync(actorId, cancellationToken))?.Email;
        }

        await directory.AppendAuditAsync(
            AuditActions.PermissionCreated,
            true,
            request.ActorId,
            actorEmail,
            null,
            null,
            row.Name,
            cancellationToken);

        return new CreatePermissionResponse(row.Name, null);
    }
}
