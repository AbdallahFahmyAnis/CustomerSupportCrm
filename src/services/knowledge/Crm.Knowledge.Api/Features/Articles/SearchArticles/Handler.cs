using Crm.Contracts.Knowledge;
using Crm.Knowledge.Api.Features.Shared;
using Crm.Knowledge.Api.Infrastructure;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.SearchArticles;

public sealed class SearchArticlesHandler(KnowledgeDb db)
    : IRequestHandler<SearchArticlesQuery, IReadOnlyList<KnowledgeArticleSummaryDto>>
{
    public Task<IReadOnlyList<KnowledgeArticleSummaryDto>> Handle(
        SearchArticlesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<KnowledgeArticleSummaryDto> items =
            db.Search(request.Q, request.Locale).Select(KnowledgeMap.Summary).ToList();
        return Task.FromResult(items);
    }
}
