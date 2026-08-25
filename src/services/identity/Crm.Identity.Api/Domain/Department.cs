namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-043 — organizational department.</summary>
public sealed class Department
{
    public static readonly Guid DemoSupportId = Guid.Parse("d1111111-1111-1111-1111-111111111111");

    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
