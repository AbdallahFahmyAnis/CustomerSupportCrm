using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.Shared;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ChangeStatus;

public sealed class ChangeStatusEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets/{id:guid}/status", async (Guid id, ChangeStatusRequest body, HttpContext http, IMediator mediator) =>
        {
            var result = await mediator.Send(new ChangeStatusCommand(id, body.Status, TicketHttp.Actor(http)));
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
