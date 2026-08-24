using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Customers.Api.Features.Bootstrap.GetBootstrapStatus;

public sealed class GetBootstrapStatusEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customers/bootstrap", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetBootstrapStatusQuery())));
    }
}
