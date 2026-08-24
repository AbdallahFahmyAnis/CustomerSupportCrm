using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.ListRoles;

public sealed class ListRolesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/roles", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new ListRolesQuery())));
    }
}
