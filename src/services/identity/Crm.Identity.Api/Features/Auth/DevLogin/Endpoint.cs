using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.DevLogin;

public sealed class DevLoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/dev-login", async (DevLoginCommand command, IMediator mediator) =>
        {
            var tokens = await mediator.Send(command);
            return tokens is null ? Results.Unauthorized() : Results.Ok(tokens);
        });
    }
}
