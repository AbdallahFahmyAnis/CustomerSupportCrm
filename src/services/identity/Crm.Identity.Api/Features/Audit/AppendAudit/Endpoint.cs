using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Audit.AppendAudit;

/// <summary>SDD CRM-036 / specs/051 — internal service ingest (no admin cookie).</summary>
public sealed class AppendAuditEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/audit", async (
            AppendAuditRequest body,
            HttpContext http,
            IMediator mediator) =>
        {
            var headerService = http.Request.Headers["X-Crm-Audit-Service"].FirstOrDefault();
            var result = await mediator.Send(new AppendAuditCommand(
                body.Action,
                body.Success,
                body.ActorEmail,
                body.TargetEmail,
                body.Detail,
                body.Service ?? headerService));
            return result.Error is null ? Results.Accepted() : Results.BadRequest(new { error = result.Error });
        });
    }
}

public sealed record AppendAuditCommand(
    string Action,
    bool Success,
    string? ActorEmail,
    string? TargetEmail,
    string? Detail,
    string? Service) : IRequest<AppendAuditResponse>;

public sealed record AppendAuditResponse(string? Error);

public sealed class AppendAuditHandler(IdentityDirectory directory)
    : IRequestHandler<AppendAuditCommand, AppendAuditResponse>
{
    public async Task<AppendAuditResponse> Handle(AppendAuditCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Action))
        {
            return new AppendAuditResponse("Action is required.");
        }

        var service = string.IsNullOrWhiteSpace(request.Service)
            ? AuditServices.Identity
            : request.Service.Trim();

        await directory.AppendAuditAsync(
            request.Action.Trim(),
            request.Success,
            null,
            request.ActorEmail,
            null,
            request.TargetEmail,
            request.Detail,
            cancellationToken,
            service);
        return new AppendAuditResponse(null);
    }
}
