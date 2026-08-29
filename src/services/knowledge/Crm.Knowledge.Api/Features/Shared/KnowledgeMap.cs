using Crm.Contracts.Knowledge;
using Crm.Knowledge.Api.Domain;

namespace Crm.Knowledge.Api.Features.Shared;

/// <summary>SDD CRM-021 — map domain ↔ contracts.</summary>
public static class KnowledgeMap
{
    public static KnowledgeArticleSummaryDto Summary(Article a) => new(
        a.Id.ToString(),
        a.Title,
        a.Kind,
        a.Status,
        a.UpdatedAt,
        a.Locale);

    public static KnowledgeArticleDetailDto Detail(Article a) => new(
        a.Id.ToString(),
        a.Title,
        a.Body,
        a.Kind,
        a.Status,
        a.CreatedBy,
        a.CreatedAt,
        a.UpdatedAt,
        a.Locale);

    public static KnowledgeSearchHitDto SearchHit(KnowledgeSearchHit hit) => new(
        hit.Id.ToString(),
        hit.Title,
        hit.Kind,
        hit.Status,
        hit.Score,
        hit.Snippet,
        hit.UpdatedAt,
        hit.Locale);
}

internal static class KnowledgeHttp
{
    public static string Actor(HttpContext http) =>
        http.Request.Headers["X-Crm-User-Email"].FirstOrDefault()
        ?? http.Request.Headers["X-Crm-User-Id"].FirstOrDefault()
        ?? "Demo Agent";
}
