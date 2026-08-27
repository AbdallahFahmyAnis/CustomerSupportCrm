using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetCsatReport;

/// <summary>SDD CRM-033</summary>
public sealed class GetCsatReportHandler(TicketsDb db)
    : IRequestHandler<GetCsatReportQuery, CsatReportDto>
{
    public Task<CsatReportDto> Handle(GetCsatReportQuery request, CancellationToken cancellationToken)
    {
        var to = request.To ?? DateTimeOffset.UtcNow;
        var from = request.From ?? to.AddDays(-30);
        if (from > to) (from, to) = (to, from);

        var rows = db.ListFeedbackForReport(from, to);
        var count = rows.Count;
        var avg = count == 0 ? 0 : Math.Round(rows.Average(r => r.Rating), 2);
        var distribution = Enumerable.Range(1, 5)
            .Select(rating => new CsatDistributionBucketDto(
                rating,
                rows.Count(r => r.Rating == rating)))
            .ToList();
        var byAgent = rows
            .GroupBy(r => new { r.AssignedAgentId, r.AssignedAgentName })
            .Select(g => new CsatAgentBucketDto(
                g.Key.AssignedAgentId,
                g.Key.AssignedAgentName,
                g.Count(),
                Math.Round(g.Average(x => x.Rating), 2)))
            .OrderByDescending(a => a.Count)
            .ToList();

        return Task.FromResult(new CsatReportDto(from, to, count, avg, distribution, byAgent));
    }
}
