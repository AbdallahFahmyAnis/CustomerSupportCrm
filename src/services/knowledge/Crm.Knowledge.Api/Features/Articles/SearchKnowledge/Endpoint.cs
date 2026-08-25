using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Knowledge.Api.Features.Articles.SearchKnowledge;

public sealed class SearchKnowledgeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/knowledge/search",
            async (string? q, string? kind, string? status, bool? publishedOnly, IMediator mediator) =>
            {
                var result = await mediator.Send(new SearchKnowledgeQuery(
                    q ?? "",
                    kind,
                    status,
                    publishedOnly ?? false));
                return result.Error is null
                    ? Results.Ok(result.Hits)
                    : Results.BadRequest(new { error = result.Error });
            });
    }
}
