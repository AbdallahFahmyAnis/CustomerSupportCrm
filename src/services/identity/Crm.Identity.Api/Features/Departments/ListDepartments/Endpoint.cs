using Crm.BuildingBlocks.Endpoints;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Departments.ListDepartments;

/// <summary>SDD CRM-043</summary>
public sealed class ListDepartmentsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/departments", async (HttpContext http, IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            return Results.Ok(await mediator.Send(new ListDepartmentsQuery()));
        });
    }
}

public sealed record ListDepartmentsQuery : IRequest<IReadOnlyList<Crm.Contracts.Identity.DepartmentDto>>;
