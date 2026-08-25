namespace Crm.Contracts.Identity;

/// <summary>SDD CRM-035 / specs/004-identity-admin.</summary>
public sealed record UserSummaryDto(
    string Id,
    string Email,
    string DisplayName,
    string Role,
    bool IsActive,
    string? DepartmentId = null,
    string? BranchId = null);

public sealed record CreateUserRequest(
    string Email,
    string DisplayName,
    string Password,
    string Role,
    string? DepartmentId = null,
    string? BranchId = null);

/// <summary>SDD CRM-043</summary>
public sealed record DepartmentDto(string Id, string Name);

public sealed record BranchDto(string Id, string DepartmentId, string Name);

public sealed record CreateDepartmentRequest(string Name);

public sealed record CreateBranchRequest(string DepartmentId, string Name);

public sealed record UpdateUserOrgRequest(string? DepartmentId, string? BranchId);

public sealed record UpdateUserRoleRequest(string Role);

public sealed record RoleSummaryDto(
    string Name,
    string Description,
    IReadOnlyList<string> Permissions);

public sealed record PermissionCatalogDto(IReadOnlyList<string> Permissions);

public sealed record CreatePermissionRequest(string Name, string? Description);

public sealed record UpdatePermissionRequest(string Name, string? Description);

public sealed record UpdateRolePermissionsRequest(IReadOnlyList<string> Permissions);

/// <summary>SDD CRM-036 / specs/011-audit-logs.</summary>
public sealed record AuditLogDto(
    string Id,
    DateTimeOffset OccurredAt,
    string Action,
    string? ActorEmail,
    string? TargetEmail,
    string? Detail,
    bool Success);

/// <summary>SDD CRM-037 / specs/012-system-config / CRM-044.</summary>
public sealed record SystemSettingsDto(
    string OrganizationName,
    string SupportEmail,
    string DefaultCulture,
    int MaxFailedLoginAttempts,
    int LockoutMinutes,
    DateTimeOffset UpdatedAt,
    string ProductTitle = "Customer Support CRM",
    string PrimaryColor = "#2563eb",
    string LogoUrl = "/brand/azm-squad.png",
    string ErpWebhookUrl = "");

public sealed record UpdateSystemSettingsRequest(
    string OrganizationName,
    string SupportEmail,
    string DefaultCulture,
    int MaxFailedLoginAttempts,
    int LockoutMinutes,
    string? ProductTitle = null,
    string? PrimaryColor = null,
    string? LogoUrl = null,
    string? ErpWebhookUrl = null);

/// <summary>SDD CRM-044 — public shell branding.</summary>
public sealed record BrandingDto(
    string ProductTitle,
    string PrimaryColor,
    string LogoUrl,
    string OrganizationName);
