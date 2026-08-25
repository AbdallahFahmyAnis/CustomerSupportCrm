using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Branches.ListBranches;

/// <summary>SDD CRM-043</summary>
public sealed class ListBranchesHandler(IdentityDirectory directory)
    : IRequestHandler<ListBranchesQuery, IReadOnlyList<BranchDto>>
{
    public Task<IReadOnlyList<BranchDto>> Handle(ListBranchesQuery request, CancellationToken cancellationToken)
        => directory.ListBranchesAsync(request.DepartmentId, cancellationToken);
}
