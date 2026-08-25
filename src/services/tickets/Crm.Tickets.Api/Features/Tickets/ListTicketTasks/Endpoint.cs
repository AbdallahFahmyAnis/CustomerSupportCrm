using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ListTicketTasks;

/// <summary>SDD CRM-014 — GET /api/tickets/{id}/tasks</summary>
public sealed class ListTicketTasksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets/{id:guid}/tasks", async (Guid id, IMediator mediator) =>
            Results.Ok(await mediator.Send(new ListTicketTasksQuery(id))));
    }
}
