using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetSlaPerformanceReport;

/// <summary>SDD CRM-032</summary>
public sealed class GetSlaPerformanceReportHandler(TicketsDb db)
    : IRequestHandler<GetSlaPerformanceReportQuery, SlaPerformanceReportDto>
{
    public Task<SlaPerformanceReportDto> Handle(
        GetSlaPerformanceReportQuery request,
        CancellationToken cancellationToken)
    {
        var to = request.To ?? DateTimeOffset.UtcNow;
        var from = request.From ?? to.AddDays(-30);
        if (from > to) (from, to) = (to, from);

        var rows = db.ListTicketsCreatedBetween(from, to);
        var asOf = to;
        var evaluated = rows.Select(r =>
        {
            var resolvedAt = ReportSlaPolicies.ResolvedAt(r.Status, r.UpdatedAt);
            var breached = ReportSlaPolicies.IsResolutionBreached(r.Priority, r.CreatedAt, resolvedAt, asOf);
            return (Row: r, Breached: breached);
        }).ToList();

        var breachedTotal = evaluated.Count(e => e.Breached);
        var byAgent = evaluated
            .GroupBy(e => new { e.Row.AssignedAgentId, e.Row.AssignedAgentName })
            .Select(g => new SlaAgentPerformanceDto(
                g.Key.AssignedAgentId,
                g.Key.AssignedAgentName,
                g.Count(),
                g.Count(x => x.Breached)))
            .OrderByDescending(a => a.ResolutionBreached)
            .ThenByDescending(a => a.TicketCount)
            .ToList();

        var pct = evaluated.Count == 0 ? 0 : Math.Round(100.0 * breachedTotal / evaluated.Count, 1);
        return Task.FromResult(new SlaPerformanceReportDto(
            from, to, evaluated.Count, breachedTotal, pct, byAgent));
    }
}
