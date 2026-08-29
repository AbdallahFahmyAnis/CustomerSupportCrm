using Crm.BuildingBlocks.Endpoints;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Audit.ListAuditLogs;

/// <summary>SDD CRM-036 / specs/051 — admin audit list (paged).</summary>
public sealed class ListAuditLogsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/audit", async (
            string? q,
            int? skip,
            int? take,
            string? service,
            HttpContext http,
            IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var result = await mediator.Send(new ListAuditLogsQuery(q, skip ?? 0, take ?? 25, service));
            return Results.Ok(result.Page);
        });
    }
}
