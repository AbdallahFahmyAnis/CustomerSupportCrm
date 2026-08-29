namespace Crm.Contracts.Knowledge;

/// <summary>SDD CRM-021 — knowledge article summary.</summary>
public sealed record KnowledgeArticleSummaryDto(
    string Id,
    string Title,
    string Kind,
    string Status,
    DateTimeOffset UpdatedAt,
    string Locale = "en");

/// <summary>SDD CRM-021 — knowledge article detail.</summary>
public sealed record KnowledgeArticleDetailDto(
    string Id,
    string Title,
    string Body,
    string Kind,
    string Status,
    string CreatedBy,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string Locale = "en");

public sealed record CreateKnowledgeArticleRequest(
    string Title,
    string Body,
    string Kind,
    string Status,
    string? Locale = null);

public sealed record UpdateKnowledgeArticleRequest(
    string Title,
    string Body,
    string Kind,
    string Status,
    string? Locale = null);

/// <summary>SDD CRM-022 — ranked knowledge search hit.</summary>
public sealed record KnowledgeSearchHitDto(
    string Id,
    string Title,
    string Kind,
    string Status,
    int Score,
    string Snippet,
    DateTimeOffset UpdatedAt,
    string Locale = "en");
