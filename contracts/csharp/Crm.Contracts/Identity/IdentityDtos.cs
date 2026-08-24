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
