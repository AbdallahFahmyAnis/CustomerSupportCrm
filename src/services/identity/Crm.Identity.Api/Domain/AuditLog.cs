namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-036 / specs/051 — append-only security audit row (EF).</summary>
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
    public string Service { get; set; } = AuditServices.Identity;
}

/// <summary>SDD CRM-036 — known audit action names.</summary>
public static class AuditActions
{
    public const string Login = "Login";
    public const string UserCreated = "UserCreated";
    public const string PasswordResetRequested = "PasswordResetRequested";
    public const string PasswordResetCompleted = "PasswordResetCompleted";
    public const string RoleChanged = "RoleChanged";
    public const string UserDeactivated = "UserDeactivated";
    public const string SettingsUpdated = "SettingsUpdated";
    public const string PermissionCreated = "PermissionCreated";
    public const string PermissionUpdated = "PermissionUpdated";
    public const string PermissionDeleted = "PermissionDeleted";
    public const string RolePermissionsUpdated = "RolePermissionsUpdated";
    public const string CustomerCreated = "CustomerCreated";
    public const string CustomerUpdated = "CustomerUpdated";
    public const string TicketCreated = "TicketCreated";
    public const string TicketStatusChanged = "TicketStatusChanged";
    public const string ArticleSaved = "ArticleSaved";
    public const string SlaPolicyUpdated = "SlaPolicyUpdated";
    public const string SlaRulesUpdated = "SlaRulesUpdated";
    public const string SlaEscalationUpdated = "SlaEscalationUpdated";
}

/// <summary>SDD CRM-036 / specs/051 — owning service labels on audit rows.</summary>
public static class AuditServices
{
    public const string Identity = "Identity";
    public const string Customers = "Customers";
    public const string Tickets = "Tickets";
    public const string Knowledge = "Knowledge";
    public const string Sla = "Sla";
    public const string Channels = "Channels";
}
