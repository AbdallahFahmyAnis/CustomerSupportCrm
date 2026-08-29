using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Audit.ListAuditLogs;

/// <summary>SDD CRM-036 / specs/051.</summary>
public sealed class ListAuditLogsHandler(IdentityDirectory directory)
    : IRequestHandler<ListAuditLogsQuery, ListAuditLogsResponse>
{
    public async Task<ListAuditLogsResponse> Handle(ListAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var (rows, total) = await directory.SearchAuditPageAsync(
            request.Q,
            request.Service,
            request.Skip,
            request.Take,
            cancellationToken);
        var items = rows
            .Select(e => new AuditLogDto(
                e.Id.ToString(),
                e.OccurredAt,
                e.Action,
                e.ActorEmail,
                e.TargetEmail,
                e.Detail,
                e.Success,
                e.Service))
            .ToList();
        return new ListAuditLogsResponse(new AuditLogPageDto(items, total, request.Skip, Math.Clamp(request.Take, 1, 100)));
    }
}
