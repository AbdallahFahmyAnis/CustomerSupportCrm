using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.SearchArticles;

public sealed class SearchArticlesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/knowledge/articles", async (string? q, string? locale, IMediator mediator) =>
            Results.Ok(await mediator.Send(new SearchArticlesQuery(q, locale))));
    }
}
