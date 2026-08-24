using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.GetPermissionCatalog;

public sealed class GetPermissionCatalogEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/permissions", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetPermissionCatalogQuery())));
    }
}
