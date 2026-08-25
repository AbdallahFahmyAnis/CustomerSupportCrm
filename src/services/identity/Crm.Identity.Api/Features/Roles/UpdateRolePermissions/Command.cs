using MediatR;

namespace Crm.Identity.Api.Features.Roles.UpdateRolePermissions;

/// <summary>SDD CRM-035 — replace permission set on a role.</summary>
public sealed record UpdateRolePermissionsCommand(
    string RoleName,
    IReadOnlyList<string> Permissions,
    Guid? ActorId) : IRequest<UpdateRolePermissionsResponse>;

public sealed record UpdateRolePermissionsResponse(
    Crm.Contracts.Identity.RoleSummaryDto? Role,
    string? Error);
