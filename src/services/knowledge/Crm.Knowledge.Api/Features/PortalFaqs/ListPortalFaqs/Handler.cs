using Crm.Contracts.Knowledge;
using Crm.Knowledge.Api.Features.Shared;
using Crm.Knowledge.Api.Infrastructure;
using MediatR;

namespace Crm.Knowledge.Api.Features.PortalFaqs.ListPortalFaqs;

/// <summary>SDD CRM-029 — list published portal FAQs.</summary>
public sealed class ListPortalFaqsHandler(KnowledgeDb db)
    : IRequestHandler<ListPortalFaqsQuery, IReadOnlyList<KnowledgeArticleSummaryDto>>
{
    public Task<IReadOnlyList<KnowledgeArticleSummaryDto>> Handle(
        ListPortalFaqsQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<KnowledgeArticleSummaryDto> items =
            db.ListPortalFaqs(request.Q).Select(KnowledgeMap.Summary).ToList();
        return Task.FromResult(items);
    }
}
