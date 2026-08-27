using Microsoft.AspNetCore.Identity;

namespace Crm.Identity.Api.Infrastructure.Persistence;

/// <summary>SDD CRM-035 / CRM-037 / CRM-043 — ASP.NET Identity user (EF Core).</summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid? DepartmentId { get; set; }
    public Guid? BranchId { get; set; }
}
