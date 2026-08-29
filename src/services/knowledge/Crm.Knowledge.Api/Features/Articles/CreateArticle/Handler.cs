using Crm.BuildingBlocks.Audit;
using Crm.Knowledge.Api.Domain;
using Crm.Knowledge.Api.Features.Shared;
using Crm.Knowledge.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Crm.Knowledge.Api.Features.Articles.CreateArticle;

/// <summary>SDD CRM-021 / CRM-036 / specs/051.</summary>
public sealed class CreateArticleHandler(
    KnowledgeDb db,
    IdentityAuditClient audit,
    IHttpContextAccessor http)
    : IRequestHandler<CreateArticleCommand, CreateArticleResponse>
{
    public async Task<CreateArticleResponse> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var article = Article.Create(
                request.Title,
                request.Body,
                request.Kind,
                request.Status,
                request.Actor,
                request.Locale);
            db.Insert(article);
            await audit.WriteAsync(
                AuditServices.Knowledge,
                "ArticleSaved",
                true,
                AuditActor.Email(http) ?? request.Actor,
                article.Id.ToString(),
                $"{article.Kind}:{article.Title}",
                cancellationToken);
            return new CreateArticleResponse(KnowledgeMap.Detail(article), null);
        }
        catch (ArgumentException ex)
        {
            return new CreateArticleResponse(null, ex.Message);
        }
    }
}
