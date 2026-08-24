using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Users.CreateUser;

public sealed class CreateUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/users", async (CreateUserRequest body, HttpContext http, IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var result = await mediator.Send(new CreateUserCommand(body.Email, body.DisplayName, body.Password, body.Role));
            return result.Error is not null
                ? Results.BadRequest(new { error = result.Error })
                : Results.Created($"/api/identity/users/{result.User!.Id}", result.User);
        });
    }
}
