namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-043 — branch under a department.</summary>
public sealed class Branch
{
    public static readonly Guid HqId = Guid.Parse("b1111111-1111-1111-1111-111111111111");
    public static readonly Guid RiyadhId = Guid.Parse("b2222222-2222-2222-2222-222222222222");
    public static readonly Guid JeddahId = Guid.Parse("b3333333-3333-3333-3333-333333333333");
    public static readonly Guid CairoId = Guid.Parse("b4444444-4444-4444-4444-444444444444");
    public static readonly Guid RemoteId = Guid.Parse("b5555555-5555-5555-5555-555555555555");
    public static readonly Guid DammamId = Guid.Parse("b6666666-6666-6666-6666-666666666666");

    public Guid Id { get; set; }
    public Guid DepartmentId { get; set; }
    public string Name { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
