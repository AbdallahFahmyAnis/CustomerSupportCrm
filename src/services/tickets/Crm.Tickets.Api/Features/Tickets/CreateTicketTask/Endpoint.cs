using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CreateTicketTask;

/// <summary>SDD CRM-014 — POST /api/tickets/{id}/tasks</summary>
public sealed class CreateTicketTaskEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets/{id:guid}/tasks", async (Guid id, CreateTicketTaskRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new CreateTicketTaskCommand(
                id, body.Title, body.DueAt, body.AssigneeUserId, body.AssigneeName));
            if (result.Error is not null)
            {
                return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? Results.NotFound(new { error = result.Error })
                    : Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/tickets/{id}/tasks/{result.Task!.Id}", result.Task);
        });
    }
}
