using Crm.Knowledge.Api.Features.Shared;
using Crm.Knowledge.Api.Infrastructure;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.UpdateArticle;

public sealed class UpdateArticleHandler(KnowledgeDb db)
    : IRequestHandler<UpdateArticleCommand, UpdateArticleResponse>
{
    public Task<UpdateArticleResponse> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        var article = db.Get(request.Id);
        if (article is null)
        {
            return Task.FromResult(new UpdateArticleResponse(null, "Article not found."));
        }

        try
        {
            article.Update(request.Title, request.Body, request.Kind, request.Status);
            db.Update(article);
            return Task.FromResult(new UpdateArticleResponse(KnowledgeMap.Detail(article), null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new UpdateArticleResponse(null, ex.Message));
        }
    }
}
