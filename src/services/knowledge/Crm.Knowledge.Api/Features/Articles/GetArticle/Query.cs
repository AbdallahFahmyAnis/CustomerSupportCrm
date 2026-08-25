using Crm.Contracts.Knowledge;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.GetArticle;

/// <summary>SDD CRM-021 — get article by id.</summary>
public sealed record GetArticleQuery(Guid Id) : IRequest<KnowledgeArticleDetailDto?>;
