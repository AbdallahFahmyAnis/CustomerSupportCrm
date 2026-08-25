using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Knowledge.Api.Features.PortalFaqs.GetPortalFaq;

/// <summary>SDD CRM-029 — GET /api/knowledge/portal/faqs/{id}.</summary>
public sealed class GetPortalFaqEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/knowledge/portal/faqs/{id:guid}", async (Guid id, IMediator mediator) =>
        {
            var article = await mediator.Send(new GetPortalFaqQuery(id));
            return article is null ? Results.NotFound() : Results.Ok(article);
        });
    }
}
