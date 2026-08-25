using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetSlaPerformanceReport;

/// <summary>SDD CRM-032 — GET /api/tickets/reports/sla-performance</summary>
public sealed class GetSlaPerformanceReportEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets/reports/sla-performance", async (
            DateTimeOffset? from,
            DateTimeOffset? to,
            IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetSlaPerformanceReportQuery(from, to))));
    }
}
