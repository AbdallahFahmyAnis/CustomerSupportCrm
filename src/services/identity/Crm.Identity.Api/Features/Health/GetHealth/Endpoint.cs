using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Identity.Api.Features.Health.GetHealth;

public sealed class GetHealthEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/health", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetHealthQuery())));
    }
}
