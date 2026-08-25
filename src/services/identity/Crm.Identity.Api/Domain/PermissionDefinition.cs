namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-035 — persisted permission catalog entry.</summary>
public sealed class PermissionDefinition
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
