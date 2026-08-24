using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.ListRoles;

public sealed class ListRolesHandler(IdentityDirectory directory)
    : IRequestHandler<ListRolesQuery, IReadOnlyList<RoleSummaryDto>>
{
    public async Task<IReadOnlyList<RoleSummaryDto>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await directory.ListRolesAsync(cancellationToken);
        return roles
            .Select(r => new RoleSummaryDto(r.Name, r.Description, r.Permissions))
            .ToList();
    }
}
