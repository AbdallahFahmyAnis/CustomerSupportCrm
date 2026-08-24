using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Identity.Api.Features.Users.SearchUsers;

public sealed class SearchUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/users", async (string? q, IMediator mediator) =>
            Results.Ok(await mediator.Send(new SearchUsersQuery(q))));
    }
}
