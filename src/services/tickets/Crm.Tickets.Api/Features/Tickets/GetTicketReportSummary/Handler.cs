using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Domain;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetTicketReportSummary;

/// <summary>SDD CRM-031</summary>
public sealed class GetTicketReportSummaryHandler(TicketsDb db)
    : IRequestHandler<GetTicketReportSummaryQuery, TicketReportSummaryDto>
{
    public Task<TicketReportSummaryDto> Handle(
        GetTicketReportSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var to = request.To ?? DateTimeOffset.UtcNow;
        var from = request.From ?? to.AddDays(-30);
        if (from > to)
        {
            (from, to) = (to, from);
        }

        var rows = db.ListTicketRowsCreatedBetween(from, to);
        var openStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            TicketStatuses.New, TicketStatuses.InProgress, TicketStatuses.Waiting
        };
        var doneStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            TicketStatuses.Resolved, TicketStatuses.Closed
        };

        var byStatus = rows.GroupBy(r => r.Status, StringComparer.OrdinalIgnoreCase)
            .Select(g => new ReportBucketDto(g.Key, g.Count()))
            .OrderByDescending(b => b.Count)
            .ToList();
        var byCategory = rows.GroupBy(r => r.Category, StringComparer.OrdinalIgnoreCase)
            .Select(g => new ReportBucketDto(g.Key, g.Count()))
            .OrderByDescending(b => b.Count)
            .ToList();
        var byPriority = rows.GroupBy(r => r.Priority, StringComparer.OrdinalIgnoreCase)
            .Select(g => new ReportBucketDto(g.Key, g.Count()))
            .OrderByDescending(b => b.Count)
            .ToList();
        var byAgent = rows
            .GroupBy(r => new { r.AssignedAgentId, r.AssignedAgentName })
            .Select(g => new ReportAgentBucketDto(g.Key.AssignedAgentId, g.Key.AssignedAgentName, g.Count()))
            .OrderByDescending(b => b.Count)
            .ToList();

        var dto = new TicketReportSummaryDto(
            from,
            to,
            rows.Count,
            rows.Count(r => openStatuses.Contains(r.Status)),
            rows.Count(r => doneStatuses.Contains(r.Status)),
            rows.Count(r => r.IsEscalated),
            byStatus,
            byCategory,
            byPriority,
            byAgent);

        return Task.FromResult(dto);
    }
}
