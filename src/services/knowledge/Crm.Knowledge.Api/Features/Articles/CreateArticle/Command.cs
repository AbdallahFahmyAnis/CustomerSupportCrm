using Crm.Contracts.Knowledge;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.CreateArticle;

/// <summary>SDD CRM-021 — create article.</summary>
public sealed record CreateArticleCommand(
    string Title,
    string Body,
    string Kind,
    string Status,
    string Actor,
    string? Locale = null) : IRequest<CreateArticleResponse>;

public sealed record CreateArticleResponse(KnowledgeArticleDetailDto? Article, string? Error);
