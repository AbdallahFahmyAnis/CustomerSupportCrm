using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Users.UpdateUserOrg;

/// <summary>SDD CRM-043 — assign department/branch on a user.</summary>
public sealed class UpdateUserOrgEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/users/{id:guid}/org", async (
            Guid id,
            UpdateUserOrgRequest body,
            HttpContext http,
            IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            Guid? dept = null;
            Guid? branch = null;
            if (!string.IsNullOrWhiteSpace(body.DepartmentId))
            {
                if (!Guid.TryParse(body.DepartmentId, out var d))
                {
                    return Results.BadRequest(new { error = "DepartmentId must be a GUID." });
                }

                dept = d;
            }

            if (!string.IsNullOrWhiteSpace(body.BranchId))
            {
                if (!Guid.TryParse(body.BranchId, out var b))
                {
                    return Results.BadRequest(new { error = "BranchId must be a GUID." });
                }

                branch = b;
            }

            var error = await mediator.Send(new UpdateUserOrgCommand(id, dept, branch));
            return error is not null ? Results.BadRequest(new { error }) : Results.NoContent();
        });
    }
}

public sealed record UpdateUserOrgCommand(Guid UserId, Guid? DepartmentId, Guid? BranchId) : IRequest<string?>;
