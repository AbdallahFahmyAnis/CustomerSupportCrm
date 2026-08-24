using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.GetPermissionCatalog;

/// <summary>SDD CRM-035.</summary>
public sealed record GetPermissionCatalogQuery : IRequest<PermissionCatalogDto>;
