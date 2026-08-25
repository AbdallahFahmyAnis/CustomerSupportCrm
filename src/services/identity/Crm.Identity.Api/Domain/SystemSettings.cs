namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-037 — singleton system settings row (EF).</summary>
public sealed class SystemSettings
{
    public static readonly Guid SingletonId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    public Guid Id { get; set; } = SingletonId;
    public string OrganizationName { get; set; } = "Customer Support CRM";
    public string SupportEmail { get; set; } = "support@crm.local";
    public string DefaultCulture { get; set; } = "en";
    public int MaxFailedLoginAttempts { get; set; } = UserAccount.MaxFailedAttempts;
    public int LockoutMinutes { get; set; } = (int)UserAccount.LockoutDuration.TotalMinutes;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
