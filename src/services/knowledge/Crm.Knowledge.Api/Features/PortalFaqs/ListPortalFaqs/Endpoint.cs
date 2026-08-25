using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Knowledge.Api.Features.PortalFaqs.ListPortalFaqs;

/// <summary>SDD CRM-029 — GET /api/knowledge/portal/faqs.</summary>
public sealed class ListPortalFaqsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/knowledge/portal/faqs", async (string? q, IMediator mediator) =>
            Results.Ok(await mediator.Send(new ListPortalFaqsQuery(q))));
    }
}
