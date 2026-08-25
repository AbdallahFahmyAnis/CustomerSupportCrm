using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetTicketReportSummary;

/// <summary>SDD CRM-031 — GET /api/tickets/reports/summary</summary>
public sealed class GetTicketReportSummaryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets/reports/summary", async (
            DateTimeOffset? from,
            DateTimeOffset? to,
            IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetTicketReportSummaryQuery(from, to))));
    }
}
