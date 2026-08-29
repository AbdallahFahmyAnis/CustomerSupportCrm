using Crm.Contracts.Knowledge;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.SearchArticles;

/// <summary>SDD CRM-021 — list/search articles.</summary>
public sealed record SearchArticlesQuery(string? Q, string? Locale = null)
    : IRequest<IReadOnlyList<KnowledgeArticleSummaryDto>>;
