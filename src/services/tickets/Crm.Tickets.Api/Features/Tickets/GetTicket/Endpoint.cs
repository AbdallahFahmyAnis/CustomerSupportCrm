using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetTicket;

public sealed class GetTicketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var ticket = await mediator.Send(new GetTicketQuery(id));
            return ticket is null ? Results.NotFound() : Results.Ok(ticket);
        });
    }
}
