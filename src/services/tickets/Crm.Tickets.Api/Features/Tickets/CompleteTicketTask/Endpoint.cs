using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CompleteTicketTask;

/// <summary>SDD CRM-014 — POST /api/tickets/{id}/tasks/{taskId}/complete</summary>
public sealed class CompleteTicketTaskEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets/{id:guid}/tasks/{taskId:guid}/complete", async (
            Guid id, Guid taskId, IMediator mediator) =>
        {
            var result = await mediator.Send(new CompleteTicketTaskCommand(id, taskId));
            if (result.Error is not null)
            {
                return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? Results.NotFound(new { error = result.Error })
                    : Results.BadRequest(new { error = result.Error });
            }

            return Results.Ok(result.Task);
        });
    }
}
