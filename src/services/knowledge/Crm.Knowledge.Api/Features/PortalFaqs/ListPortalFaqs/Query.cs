using Crm.Contracts.Knowledge;
using MediatR;

namespace Crm.Knowledge.Api.Features.PortalFaqs.ListPortalFaqs;

/// <summary>SDD CRM-029 — list published portal FAQs.</summary>
public sealed record ListPortalFaqsQuery(string? Q) : IRequest<IReadOnlyList<KnowledgeArticleSummaryDto>>;
