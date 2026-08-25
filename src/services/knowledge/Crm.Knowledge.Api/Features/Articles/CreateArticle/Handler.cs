using Crm.Knowledge.Api.Domain;
using Crm.Knowledge.Api.Features.Shared;
using Crm.Knowledge.Api.Infrastructure;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.CreateArticle;

public sealed class CreateArticleHandler(KnowledgeDb db)
    : IRequestHandler<CreateArticleCommand, CreateArticleResponse>
{
    public Task<CreateArticleResponse> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var article = Article.Create(
                request.Title,
                request.Body,
                request.Kind,
                request.Status,
                request.Actor);
            db.Insert(article);
            return Task.FromResult(new CreateArticleResponse(KnowledgeMap.Detail(article), null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new CreateArticleResponse(null, ex.Message));
        }
    }
}
