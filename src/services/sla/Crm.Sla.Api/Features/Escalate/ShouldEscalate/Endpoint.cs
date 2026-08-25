using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Escalate.ShouldEscalate;

public sealed class ShouldEscalateEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/sla/should-escalate", async (ShouldEscalateRequest body, IMediator mediator) =>
            Results.Ok(await mediator.Send(new ShouldEscalateQuery(
                body.Priority,
                body.CreatedAt,
                body.IsEscalated,
                body.Status,
                body.AssignedAgentId,
                body.FirstResponseAt,
                body.ResolvedAt,
                body.AsOf))));
    }
}
