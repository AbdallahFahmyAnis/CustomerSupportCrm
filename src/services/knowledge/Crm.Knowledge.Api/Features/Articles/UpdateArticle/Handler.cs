using Crm.BuildingBlocks.Audit;
using Crm.Knowledge.Api.Features.Shared;
using Crm.Knowledge.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Crm.Knowledge.Api.Features.Articles.UpdateArticle;

/// <summary>SDD CRM-021 / CRM-036 / specs/051.</summary>
public sealed class UpdateArticleHandler(
    KnowledgeDb db,
    IdentityAuditClient audit,
    IHttpContextAccessor http)
    : IRequestHandler<UpdateArticleCommand, UpdateArticleResponse>
{
    public async Task<UpdateArticleResponse> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
    {
        var article = db.Get(request.Id);
        if (article is null)
        {
            return new UpdateArticleResponse(null, "Article not found.");
        }

        try
        {
            article.Update(request.Title, request.Body, request.Kind, request.Status, request.Locale);
            db.Update(article);
            await audit.WriteAsync(
                AuditServices.Knowledge,
                "ArticleSaved",
                true,
                AuditActor.Email(http),
                article.Id.ToString(),
                $"{article.Kind}:{article.Title} ({article.Status})",
                cancellationToken);
            return new UpdateArticleResponse(KnowledgeMap.Detail(article), null);
        }
        catch (ArgumentException ex)
        {
            return new UpdateArticleResponse(null, ex.Message);
        }
    }
}
