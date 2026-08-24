using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Customers;
using MediatR;

namespace Crm.Customers.Api.Features.Contacts.AddContact;

public sealed class AddContactEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customers/{id:guid}/contacts", async (Guid id, AddContactRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new AddContactCommand(id, body.Type, body.Value, body.IsPrimary));
            if (result.Error is not null)
            {
                return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? Results.NotFound(new { error = result.Error })
                    : Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/customers/{id}/contacts/{result.Contact!.Id}", result.Contact);
        });
    }
}
