using Crm.BuildingBlocks.Endpoints;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Audit.GetAuditLog;

/// <summary>SDD CRM-036 / specs/051 — admin audit event detail.</summary>
public sealed class GetAuditLogEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/audit/{id:guid}", async (
            Guid id,
            HttpContext http,
            IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var row = await mediator.Send(new GetAuditLogQuery(id));
            return row is null ? Results.NotFound() : Results.Ok(row);
        });
    }
}
