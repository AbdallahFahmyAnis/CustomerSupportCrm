using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Audit.ListAuditLogs;

public sealed class ListAuditLogsHandler(IdentityDirectory directory)
    : IRequestHandler<ListAuditLogsQuery, ListAuditLogsResponse>
{
    public async Task<ListAuditLogsResponse> Handle(ListAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var rows = await directory.SearchAuditAsync(request.Q, request.Take, cancellationToken);
        var items = rows
            .Select(e => new AuditLogDto(
                e.Id.ToString(),
                e.OccurredAt,
                e.Action,
                e.ActorEmail,
                e.TargetEmail,
                e.Detail,
                e.Success))
            .ToList();
        return new ListAuditLogsResponse(items);
    }
}
