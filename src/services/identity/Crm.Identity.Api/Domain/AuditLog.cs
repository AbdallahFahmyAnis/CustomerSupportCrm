namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-036 — append-only security audit row (EF).</summary>
public sealed class AuditLogEntry
{
    public Guid Id { get; set; }
    public DateTimeOffset OccurredAt { get; set; }
    public string Action { get; set; } = "";
    public Guid? ActorUserId { get; set; }
    public string? ActorEmail { get; set; }
    public Guid? TargetUserId { get; set; }
    public string? TargetEmail { get; set; }
    public string? Detail { get; set; }
    public bool Success { get; set; }
}

/// <summary>SDD CRM-036 — known audit action names.</summary>
public static class AuditActions
{
    public const string Login = "Login";
    public const string UserCreated = "UserCreated";
    public const string RoleChanged = "RoleChanged";
    public const string UserDeactivated = "UserDeactivated";
    public const string SettingsUpdated = "SettingsUpdated";
}
