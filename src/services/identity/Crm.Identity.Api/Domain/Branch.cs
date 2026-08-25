namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-043 — branch under a department.</summary>
public sealed class Branch
{
    public static readonly Guid HqId = Guid.Parse("b1111111-1111-1111-1111-111111111111");
    public static readonly Guid RiyadhId = Guid.Parse("b2222222-2222-2222-2222-222222222222");

    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public string Name { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
