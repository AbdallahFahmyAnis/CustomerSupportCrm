using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Departments.ListDepartments;

/// <summary>SDD CRM-043</summary>
public sealed class ListDepartmentsHandler(IdentityDirectory directory)
    : IRequestHandler<ListDepartmentsQuery, IReadOnlyList<DepartmentDto>>
{
    public Task<IReadOnlyList<DepartmentDto>> Handle(ListDepartmentsQuery request, CancellationToken cancellationToken)
        => directory.ListDepartmentsAsync(cancellationToken);
}
