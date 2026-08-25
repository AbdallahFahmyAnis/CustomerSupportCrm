using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.Shared;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.AddTicketNote;

/// <summary>SDD CRM-016 — POST /api/tickets/{id}/notes.</summary>
public sealed class AddTicketNoteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets/{id:guid}/notes", async (
            Guid id,
            AddTicketNoteRequest body,
            HttpContext http,
            IMediator mediator) =>
        {
            var result = await mediator.Send(new AddTicketNoteCommand(
                id,
                body.Body,
                TicketHttp.Actor(http),
                TicketHttp.ActorUserId(http)));
            if (result.Error is not null)
            {
                return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? Results.NotFound(new { error = result.Error })
                    : Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/tickets/{id}/notes/{result.Note!.Id}", result.Note);
        });
    }
}
