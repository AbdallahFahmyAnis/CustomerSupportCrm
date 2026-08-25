using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.CreatePermission;

public sealed class CreatePermissionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/permissions", async (
            CreatePermissionRequest body,
            HttpContext http,
            IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var result = await mediator.Send(new CreatePermissionCommand(
                body.Name,
                body.Description,
                AdminHttp.ActorId(http)));
            return result.Error is not null
                ? Results.BadRequest(new { error = result.Error })
                : Results.Created($"/api/identity/permissions/{result.Name}", new { name = result.Name });
        });
    }
}
