using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.GetArticle;

public sealed class GetArticleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/knowledge/articles/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var article = await mediator.Send(new GetArticleQuery(id));
            return article is null ? Results.NotFound() : Results.Ok(article);
        });
    }
}
