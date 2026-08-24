using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.GetPermissionCatalog;

public sealed class GetPermissionCatalogHandler : IRequestHandler<GetPermissionCatalogQuery, PermissionCatalogDto>
{
    public Task<PermissionCatalogDto> Handle(GetPermissionCatalogQuery request, CancellationToken cancellationToken)
        => Task.FromResult(new PermissionCatalogDto(PermissionCatalog.All));
}
