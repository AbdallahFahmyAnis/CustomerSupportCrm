using Crm.BuildingBlocks.Endpoints;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.DeletePermission;

public sealed class DeletePermissionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/identity/permissions/{name}", async (
            string name,
            HttpContext http,
            IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var result = await mediator.Send(new DeletePermissionCommand(name, AdminHttp.ActorId(http)));
            return result.Error is not null
                ? Results.BadRequest(new { error = result.Error })
                : Results.NoContent();
        });
    }
}
