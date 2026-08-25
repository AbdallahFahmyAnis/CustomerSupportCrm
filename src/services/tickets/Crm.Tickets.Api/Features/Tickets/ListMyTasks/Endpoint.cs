using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ListMyTasks;

/// <summary>SDD CRM-014 — GET /api/tickets/tasks</summary>
public sealed class ListMyTasksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets/tasks", async (string? assignedTo, DateTimeOffset? dueBefore, IMediator mediator) =>
            Results.Ok(await mediator.Send(new ListMyTasksQuery(assignedTo, dueBefore))));
    }
}
