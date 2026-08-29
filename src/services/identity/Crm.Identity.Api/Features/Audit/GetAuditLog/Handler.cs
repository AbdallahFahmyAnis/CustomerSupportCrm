using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Audit.GetAuditLog;

/// <summary>SDD CRM-036 / specs/051.</summary>
public sealed class GetAuditLogHandler(IdentityDirectory directory)
    : IRequestHandler<GetAuditLogQuery, AuditLogDetailDto?>
{
    public async Task<AuditLogDetailDto?> Handle(
        GetAuditLogQuery request,
        CancellationToken cancellationToken)
    {
        var row = await directory.GetAuditByIdAsync(request.Id, cancellationToken);
        if (row is null)
        {
            return null;
        }

        string? actorName = null;
        if (row.ActorUserId is Guid actorId)
        {
            actorName = (await directory.GetUserAsync(actorId, cancellationToken))?.DisplayName;
        }

        string? targetName = null;
        if (row.TargetUserId is Guid targetId)
        {
            targetName = (await directory.GetUserAsync(targetId, cancellationToken))?.DisplayName;
        }

        return new AuditLogDetailDto(
            row.Id.ToString(),
            row.OccurredAt,
            row.Action,
            row.Service,
            row.Success,
            row.Detail,
            row.ActorUserId?.ToString(),
            actorName,
            row.ActorEmail,
            row.TargetUserId?.ToString(),
            targetName,
            row.TargetEmail);
    }
}
