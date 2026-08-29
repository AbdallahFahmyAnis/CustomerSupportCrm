using Crm.Contracts.Knowledge;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.UpdateArticle;

/// <summary>SDD CRM-021 — update article.</summary>
public sealed record UpdateArticleCommand(
    Guid Id,
    string Title,
    string Body,
    string Kind,
    string Status,
    string? Locale = null) : IRequest<UpdateArticleResponse>;

public sealed record UpdateArticleResponse(KnowledgeArticleDetailDto? Article, string? Error);
