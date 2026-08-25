using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.GetPermissionCatalog;

/// <summary>SDD CRM-035 — list permission catalog from store.</summary>
public sealed class GetPermissionCatalogHandler(IdentityDirectory directory)
    : IRequestHandler<GetPermissionCatalogQuery, PermissionCatalogDto>
{
    public async Task<PermissionCatalogDto> Handle(
        GetPermissionCatalogQuery request,
        CancellationToken cancellationToken)
    {
        var names = await directory.ListPermissionNamesAsync(cancellationToken);
        if (names.Count == 0)
        {
            return new PermissionCatalogDto(Domain.PermissionCatalog.All);
        }

        return new PermissionCatalogDto(names);
    }
}
