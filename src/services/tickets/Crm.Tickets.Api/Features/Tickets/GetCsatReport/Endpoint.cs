using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetCsatReport;

/// <summary>SDD CRM-033 — GET /api/tickets/reports/csat</summary>
public sealed class GetCsatReportEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets/reports/csat", async (
            DateTimeOffset? from,
            DateTimeOffset? to,
            IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetCsatReportQuery(from, to))));
    }
}
