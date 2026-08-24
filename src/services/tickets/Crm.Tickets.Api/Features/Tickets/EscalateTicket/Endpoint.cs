using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.Shared;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.EscalateTicket;

public sealed class EscalateTicketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets/{id:guid}/escalate", async (Guid id, EscalateTicketRequest body, HttpContext http, IMediator mediator) =>
        {
            var result = await mediator.Send(new EscalateTicketCommand(
                id, body.AssignToAgentId, body.AssignToAgentName, TicketHttp.Actor(http)));
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
