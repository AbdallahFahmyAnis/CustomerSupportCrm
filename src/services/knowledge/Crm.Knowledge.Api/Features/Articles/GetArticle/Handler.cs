using Crm.Contracts.Knowledge;
using Crm.Knowledge.Api.Features.Shared;
using Crm.Knowledge.Api.Infrastructure;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.GetArticle;

public sealed class GetArticleHandler(KnowledgeDb db)
    : IRequestHandler<GetArticleQuery, KnowledgeArticleDetailDto?>
{
    public Task<KnowledgeArticleDetailDto?> Handle(GetArticleQuery request, CancellationToken cancellationToken)
    {
        var article = db.Get(request.Id);
        return Task.FromResult(article is null ? null : KnowledgeMap.Detail(article));
    }
}
