using Crm.Contracts.Knowledge;
using MediatR;

namespace Crm.Knowledge.Api.Features.PortalFaqs.GetPortalFaq;

/// <summary>SDD CRM-029 — get one published portal FAQ.</summary>
public sealed record GetPortalFaqQuery(Guid Id) : IRequest<KnowledgeArticleDetailDto?>;
