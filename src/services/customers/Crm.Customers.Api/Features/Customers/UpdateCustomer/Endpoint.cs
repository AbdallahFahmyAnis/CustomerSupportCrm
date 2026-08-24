using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Customers;
using MediatR;

namespace Crm.Customers.Api.Features.Customers.UpdateCustomer;

public sealed class UpdateCustomerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/customers/{id:guid}", async (Guid id, UpdateCustomerRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateCustomerCommand(
                id,
                body.DisplayName,
                body.UniqueIdentifier,
                body.Organization,
                body.Status));
            if (result.Error is not null)
            {
                return Results.NotFound(new { error = result.Error });
            }

            if (result.Duplicate is not null)
            {
                return Results.Conflict(result.Duplicate);
            }

            return Results.Ok(result.Customer);
        });
    }
}
