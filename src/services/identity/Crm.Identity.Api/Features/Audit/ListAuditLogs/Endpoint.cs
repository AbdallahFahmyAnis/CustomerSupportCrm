using Crm.BuildingBlocks.Endpoints;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Audit.ListAuditLogs;

/// <summary>SDD CRM-036 — admin audit list.</summary>
public sealed class ListAuditLogsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/audit", async (string? q, int? take, HttpContext http, IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var result = await mediator.Send(new ListAuditLogsQuery(q, take ?? 100));
            return Results.Ok(result.Items);
        });
    }
}
