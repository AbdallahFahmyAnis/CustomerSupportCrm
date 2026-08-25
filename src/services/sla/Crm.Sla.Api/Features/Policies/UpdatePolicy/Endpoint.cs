using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Policies.UpdatePolicy;

public sealed class UpdatePolicyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/sla/policies/{priority}",
            async (string priority, UpdateSlaPolicyRequest body, IMediator mediator) =>
            {
                var result = await mediator.Send(
                    new UpdatePolicyCommand(priority, body.FirstResponseMinutes, body.ResolutionMinutes));
                if (result.Error is null)
                {
                    return Results.Ok(result.Policy);
                }

                return result.Error.Contains("Unknown", StringComparison.OrdinalIgnoreCase)
                    ? Results.NotFound(new { error = result.Error })
                    : Results.BadRequest(new { error = result.Error });
            });
    }
}
