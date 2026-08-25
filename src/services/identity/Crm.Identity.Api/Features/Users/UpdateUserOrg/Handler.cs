using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Users.UpdateUserOrg;

/// <summary>SDD CRM-043</summary>
public sealed class UpdateUserOrgHandler(IdentityDirectory directory)
    : IRequestHandler<UpdateUserOrgCommand, string?>
{
    public Task<string?> Handle(UpdateUserOrgCommand request, CancellationToken cancellationToken)
        => directory.AssignUserOrgAsync(request.UserId, request.DepartmentId, request.BranchId, cancellationToken);
}
