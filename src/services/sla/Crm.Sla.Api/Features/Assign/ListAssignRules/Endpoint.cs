using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Sla.Api.Features.Assign.ListAssignRules;

public sealed class ListAssignRulesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sla/assign-rules", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new ListAssignRulesQuery())));
    }
}
