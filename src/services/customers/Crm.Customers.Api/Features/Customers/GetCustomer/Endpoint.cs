using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Customers.Api.Features.Customers.GetCustomer;

public sealed class GetCustomerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customers/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var customer = await mediator.Send(new GetCustomerQuery(id));
            return customer is null ? Results.NotFound() : Results.Ok(customer);
        });
    }
}
