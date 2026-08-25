using Crm.Knowledge.Api.Features.Shared;
using Crm.Knowledge.Api.Infrastructure;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.SearchKnowledge;

public sealed class SearchKnowledgeHandler(KnowledgeDb db)
    : IRequestHandler<SearchKnowledgeQuery, SearchKnowledgeResponse>
{
    public Task<SearchKnowledgeResponse> Handle(SearchKnowledgeQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Q))
        {
            return Task.FromResult(new SearchKnowledgeResponse(null, "Query q is required."));
        }

        IReadOnlyList<Crm.Contracts.Knowledge.KnowledgeSearchHitDto> hits = db
            .RankedSearch(request.Q, request.Kind, request.Status, request.PublishedOnly)
            .Select(KnowledgeMap.SearchHit)
            .ToList();
        return Task.FromResult(new SearchKnowledgeResponse(hits, null));
    }
}
