using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Branches.CreateBranch;

/// <summary>SDD CRM-043</summary>
public sealed class CreateBranchEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/branches", async (
            CreateBranchRequest body,
            HttpContext http,
            IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            if (!Guid.TryParse(body.DepartmentId, out var deptId))
            {
                return Results.BadRequest(new { error = "DepartmentId must be a GUID." });
            }

            var result = await mediator.Send(new CreateBranchCommand(deptId, body.Name));
            return result.Error is not null
                ? Results.BadRequest(new { error = result.Error })
                : Results.Created($"/api/identity/branches/{result.Branch!.Id}", result.Branch);
        });
    }
}

public sealed record CreateBranchCommand(Guid DepartmentId, string Name) : IRequest<CreateBranchResponse>;
public sealed record CreateBranchResponse(BranchDto? Branch, string? Error);
