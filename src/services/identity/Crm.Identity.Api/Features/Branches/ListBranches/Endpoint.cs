using Crm.BuildingBlocks.Endpoints;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Branches.ListBranches;

/// <summary>SDD CRM-043</summary>
public sealed class ListBranchesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/branches", async (string? departmentId, HttpContext http, IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            Guid? dept = null;
            if (!string.IsNullOrWhiteSpace(departmentId) && Guid.TryParse(departmentId, out var id))
            {
                dept = id;
            }

            return Results.Ok(await mediator.Send(new ListBranchesQuery(dept)));
        });
    }
}

public sealed record ListBranchesQuery(Guid? DepartmentId)
    : IRequest<IReadOnlyList<Crm.Contracts.Identity.BranchDto>>;
