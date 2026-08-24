using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.IssueToken;

public sealed class IssueTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/token", async (DevLoginRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new IssueTokenCommand(body.Email, body.Password));
            if (result.Tokens is not null)
            {
                return Results.Ok(result.Tokens);
            }

            return result.Locked
                ? Results.Json(new { error = result.Error }, statusCode: StatusCodes.Status423Locked)
                : Results.Json(new { error = result.Error ?? "Unauthorized" }, statusCode: StatusCodes.Status401Unauthorized);
        });
    }
}
