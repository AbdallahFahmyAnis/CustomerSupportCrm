using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.RotateRefreshToken;

public sealed class RotateRefreshTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/token/refresh", async (RefreshTokenRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new RotateRefreshTokenCommand(body.RefreshToken));
            return result.Tokens is not null
                ? Results.Ok(result.Tokens)
                : Results.Json(new { error = result.Error ?? "Unauthorized" }, statusCode: StatusCodes.Status401Unauthorized);
        });
    }
}
