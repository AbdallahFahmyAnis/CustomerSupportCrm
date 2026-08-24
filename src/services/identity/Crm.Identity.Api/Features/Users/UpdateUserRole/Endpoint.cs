using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Users.UpdateUserRole;

public sealed class UpdateUserRoleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/users/{id:guid}/role", async (Guid id, UpdateUserRoleRequest body, HttpContext http, IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var result = await mediator.Send(new UpdateUserRoleCommand(id, body.Role));
            if (result.Error is null)
            {
                return Results.Ok(result.User);
            }

            return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? Results.NotFound(new { error = result.Error })
                : Results.BadRequest(new { error = result.Error });
        });
    }
}
