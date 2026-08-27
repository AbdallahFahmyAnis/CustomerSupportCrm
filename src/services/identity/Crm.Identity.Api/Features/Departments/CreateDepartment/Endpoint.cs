using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Departments.CreateDepartment;

/// <summary>SDD CRM-043</summary>
public sealed class CreateDepartmentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/departments", async (
            CreateDepartmentRequest body,
            HttpContext http,
            IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var result = await mediator.Send(new CreateDepartmentCommand(body.Name));
            return result.Error is not null
                ? Results.BadRequest(new { error = result.Error })
                : Results.Created($"/api/identity/departments/{result.Department!.Id}", result.Department);
        });
    }
}

public sealed record CreateDepartmentCommand(string Name) : IRequest<CreateDepartmentResponse>;
public sealed record CreateDepartmentResponse(DepartmentDto? Department, string? Error);
