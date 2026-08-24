using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.Shared;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.AssignTicket;

public sealed class AssignTicketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets/{id:guid}/assign", async (Guid id, AssignTicketRequest body, HttpContext http, IMediator mediator) =>
        {
            var result = await mediator.Send(new AssignTicketCommand(id, body.AgentId, body.AgentName, TicketHttp.Actor(http)));
            if (result.Error is null)
            {
                return Results.Ok(result.Ticket);
            }

            return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? Results.NotFound(new { error = result.Error })
                : Results.BadRequest(new { error = result.Error });
        });
    }
}
