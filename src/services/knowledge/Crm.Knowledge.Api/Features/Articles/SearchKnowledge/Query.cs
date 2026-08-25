using Crm.Contracts.Knowledge;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.SearchKnowledge;

/// <summary>SDD CRM-022 — ranked knowledge search.</summary>
public sealed record SearchKnowledgeQuery(
    string Q,
    string? Kind,
    string? Status,
    bool PublishedOnly) : IRequest<SearchKnowledgeResponse>;

public sealed record SearchKnowledgeResponse(IReadOnlyList<KnowledgeSearchHitDto>? Hits, string? Error);
