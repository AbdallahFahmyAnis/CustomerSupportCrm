using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Customers.Api.Features.Contacts.DeactivateContact;

public sealed class DeactivateContactEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customers/{id:guid}/contacts/{contactId:guid}/deactivate",
            async (Guid id, Guid contactId, IMediator mediator) =>
            {
                var result = await mediator.Send(new DeactivateContactCommand(id, contactId));
                return result.Ok ? Results.NoContent() : Results.NotFound(new { error = result.Error });
            });
    }
}
