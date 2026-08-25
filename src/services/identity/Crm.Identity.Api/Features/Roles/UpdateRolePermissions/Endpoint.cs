using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.UpdateRolePermissions;

public sealed class UpdateRolePermissionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/identity/roles/{name}/permissions", async (
            string name,
            UpdateRolePermissionsRequest body,
            HttpContext http,
            IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var result = await mediator.Send(new UpdateRolePermissionsCommand(
                name,
                body.Permissions ?? [],
                AdminHttp.ActorId(http)));
            return result.Error is not null
                ? Results.BadRequest(new { error = result.Error })
                : Results.Ok(result.Role);
        });
    }
}
