using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Knowledge;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.UpdateArticle;

public sealed class UpdateArticleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/knowledge/articles/{id:guid}",
            async (Guid id, UpdateKnowledgeArticleRequest body, IMediator mediator) =>
            {
                var result = await mediator.Send(new UpdateArticleCommand(
                    id,
                    body.Title,
                    body.Body,
                    body.Kind,
                    body.Status,
                    body.Locale));
                if (result.Error is null)
                {
                    return Results.Ok(result.Article);
                }

                return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? Results.NotFound(new { error = result.Error })
                    : Results.BadRequest(new { error = result.Error });
            });
    }
}
