using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Customers.Api.Features.Customers.SearchCustomers;

public sealed class SearchCustomersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customers", async (string? q, IMediator mediator) =>
            Results.Ok(await mediator.Send(new SearchCustomersQuery(q))));
    }
}
