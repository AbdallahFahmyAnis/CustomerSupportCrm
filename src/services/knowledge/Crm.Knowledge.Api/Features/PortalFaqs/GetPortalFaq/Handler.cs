using Crm.Contracts.Knowledge;
using Crm.Knowledge.Api.Features.Shared;
using Crm.Knowledge.Api.Infrastructure;
using MediatR;

namespace Crm.Knowledge.Api.Features.PortalFaqs.GetPortalFaq;

/// <summary>SDD CRM-029 — get one published portal FAQ.</summary>
public sealed class GetPortalFaqHandler(KnowledgeDb db)
    : IRequestHandler<GetPortalFaqQuery, KnowledgeArticleDetailDto?>
{
    public Task<KnowledgeArticleDetailDto?> Handle(GetPortalFaqQuery request, CancellationToken cancellationToken)
    {
        var article = db.GetPortalFaq(request.Id);
        return Task.FromResult(article is null ? null : KnowledgeMap.Detail(article));
    }
}
