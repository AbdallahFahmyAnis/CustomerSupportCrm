using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.ListRoles;

public sealed class ListRolesHandler(IdentityDb db) : IRequestHandler<ListRolesQuery, IReadOnlyList<RoleSummaryDto>>
{
    public Task<IReadOnlyList<RoleSummaryDto>> Handle(ListRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = db.ListRoles()
            .Select(r => new RoleSummaryDto(r.Name, r.Description, r.Permissions))
            .ToList();
        return Task.FromResult<IReadOnlyList<RoleSummaryDto>>(roles);
    }
}
