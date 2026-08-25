using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Assign.ReplaceAssignRules;

public sealed class ReplaceAssignRulesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/sla/assign-rules", async (ReplaceAutoAssignRulesRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new ReplaceAssignRulesCommand(body.Rules));
            return result.Error is null
                ? Results.Ok(result.Rules)
                : Results.BadRequest(new { error = result.Error });
        });
    }
}
