using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.SearchTickets;

public sealed class SearchTicketsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets", async (string? q, string? assignedTo, string? departmentId, IMediator mediator) =>
            Results.Ok(await mediator.Send(new SearchTicketsQuery(q, assignedTo, departmentId))));
    }
}
