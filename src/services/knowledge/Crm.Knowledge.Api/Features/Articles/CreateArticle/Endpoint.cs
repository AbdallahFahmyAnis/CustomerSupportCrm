using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Knowledge;
using Crm.Knowledge.Api.Features.Shared;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.CreateArticle;

public sealed class CreateArticleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/knowledge/articles",
            async (CreateKnowledgeArticleRequest body, HttpContext http, IMediator mediator) =>
            {
                var result = await mediator.Send(new CreateArticleCommand(
                    body.Title,
                    body.Body,
                    body.Kind,
                    body.Status,
                    KnowledgeHttp.Actor(http)));
                return result.Error is null
                    ? Results.Ok(result.Article)
                    : Results.BadRequest(new { error = result.Error });
            });
    }
}
