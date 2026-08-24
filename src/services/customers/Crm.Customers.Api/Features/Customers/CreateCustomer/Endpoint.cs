using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Customers;
using MediatR;

namespace Crm.Customers.Api.Features.Customers.CreateCustomer;

public sealed class CreateCustomerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customers", async (CreateCustomerRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new CreateCustomerCommand(
                body.DisplayName,
                body.UniqueIdentifier,
                body.Organization,
                body.Status));
            if (result.Duplicate is not null)
            {
                return Results.Conflict(result.Duplicate);
            }

            return Results.Created($"/api/customers/{result.Customer!.Id}", result.Customer);
        });
    }
}
