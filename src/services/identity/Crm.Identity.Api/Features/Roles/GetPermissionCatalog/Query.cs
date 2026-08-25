using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.GetPermissionCatalog;

public sealed record GetPermissionCatalogQuery : IRequest<PermissionCatalogDto>;
