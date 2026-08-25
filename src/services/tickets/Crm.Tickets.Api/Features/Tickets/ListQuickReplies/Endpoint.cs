using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ListQuickReplies;

/// <summary>SDD CRM-015 — GET /api/tickets/quick-replies</summary>
public sealed class ListQuickRepliesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets/quick-replies", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new ListQuickRepliesQuery())));
    }
}
