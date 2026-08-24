using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Roles.ListRoles;

/// <summary>SDD CRM-035.</summary>
public sealed record ListRolesQuery : IRequest<IReadOnlyList<RoleSummaryDto>>;
