using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Departments.CreateDepartment;

/// <summary>SDD CRM-043</summary>
public sealed class CreateDepartmentHandler(IdentityDirectory directory)
    : IRequestHandler<CreateDepartmentCommand, CreateDepartmentResponse>
{
    public async Task<CreateDepartmentResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        var (dept, error) = await directory.CreateDepartmentAsync(request.Name, cancellationToken);
        return new CreateDepartmentResponse(dept, error);
    }
}
