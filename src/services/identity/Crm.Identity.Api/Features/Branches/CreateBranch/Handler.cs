using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Branches.CreateBranch;

/// <summary>SDD CRM-043</summary>
public sealed class CreateBranchHandler(IdentityDirectory directory)
    : IRequestHandler<CreateBranchCommand, CreateBranchResponse>
{
    public async Task<CreateBranchResponse> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var (branch, error) = await directory.CreateBranchAsync(request.DepartmentId, request.Name, cancellationToken);
        return new CreateBranchResponse(branch, error);
    }
}
