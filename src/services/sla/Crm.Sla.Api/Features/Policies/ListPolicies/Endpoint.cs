using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Sla.Api.Features.Policies.ListPolicies;

public sealed class ListPoliciesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sla/policies", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new ListPoliciesQuery())));
    }
}
