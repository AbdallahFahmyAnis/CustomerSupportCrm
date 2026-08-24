using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.RevokeToken;

public sealed class RevokeTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/token/revoke", async (RevokeTokenRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new RevokeTokenCommand(body.RefreshToken, body.AccessToken));
            return result.Ok ? Results.NoContent() : Results.BadRequest(new { error = result.Error });
        });
    }
}
