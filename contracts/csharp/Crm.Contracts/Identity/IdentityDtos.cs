namespace Crm.Contracts.Identity;

/// <summary>SDD CRM-035 / specs/004-identity-admin.</summary>
public sealed record UserSummaryDto(
    string Id,
    string Email,
    string DisplayName,
    string Role,
    bool IsActive);

public sealed record CreateUserRequest(
    string Email,
    string DisplayName,
    string Password,
    string Role);

public sealed record UpdateUserRoleRequest(string Role);

public sealed record RoleSummaryDto(
    string Name,
    string Description,
    IReadOnlyList<string> Permissions);

public sealed record PermissionCatalogDto(IReadOnlyList<string> Permissions);

/// <summary>SDD CRM-036 / specs/011-audit-logs.</summary>
public sealed record AuditLogDto(
    string Id,
    DateTimeOffset OccurredAt,
    string Action,
    string? ActorEmail,
    string? TargetEmail,
    string? Detail,
    bool Success);
