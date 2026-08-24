using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.Shared;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CreateTicket;

public sealed class CreateTicketEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets", async (CreateTicketRequest body, HttpContext http, IMediator mediator) =>
        {
            if (!Guid.TryParse(body.CustomerId, out var customerId))
            {
                return Results.BadRequest(new { error = "CustomerId must be a GUID." });
            }

            var result = await mediator.Send(new CreateTicketCommand(
                customerId,
                body.CustomerName,
                body.Subject,
                body.Description,
                body.Category,
                body.Priority,
                TicketHttp.Actor(http)));
            return result.Error is not null
                ? Results.BadRequest(new { error = result.Error })
                : Results.Created($"/api/tickets/{result.Ticket!.Id}", result.Ticket);
        });
    }
}
