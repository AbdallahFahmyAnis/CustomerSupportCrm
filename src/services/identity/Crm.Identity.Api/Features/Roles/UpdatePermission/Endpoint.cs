using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.UpdatePermission;

public sealed class UpdatePermissionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/identity/permissions/{name}", async (
            string name,
            UpdatePermissionRequest body,
            HttpContext http,
            IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var result = await mediator.Send(new UpdatePermissionCommand(
                name,
                body.Name,
                body.Description,
                AdminHttp.ActorId(http)));
            return result.Error is not null
                ? Results.BadRequest(new { error = result.Error })
                : Results.Ok(new { name = result.Name });
        });
    }
}
