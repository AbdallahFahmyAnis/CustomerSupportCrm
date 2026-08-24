using Crm.BuildingBlocks.Endpoints;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Users.DeactivateUser;

public sealed class DeactivateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/users/{id:guid}/deactivate", async (Guid id, HttpContext http, IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var result = await mediator.Send(new DeactivateUserCommand(id, AdminHttp.ActorId(http)));
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
