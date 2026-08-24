using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.ListRoles;

/// <summary>SDD CRM-035.</summary>
public sealed record ListRolesQuery : IRequest<IReadOnlyList<RoleSummaryDto>>;

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

public sealed record GetPermissionCatalogQuery : IRequest<PermissionCatalogDto>;

public sealed class GetPermissionCatalogHandler : IRequestHandler<GetPermissionCatalogQuery, PermissionCatalogDto>
{
    public Task<PermissionCatalogDto> Handle(GetPermissionCatalogQuery request, CancellationToken cancellationToken)
        => Task.FromResult(new PermissionCatalogDto(PermissionCatalog.All));
}
