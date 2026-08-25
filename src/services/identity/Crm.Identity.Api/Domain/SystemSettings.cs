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
    /// <summary>SDD CRM-044</summary>
    public string ProductTitle { get; set; } = "Customer Support CRM";
    public string PrimaryColor { get; set; } = "#2563eb";
    public string LogoUrl { get; set; } = "/brand/azm-squad.png";
    /// <summary>SDD CRM-039 — empty disables ERP webhook.</summary>
    public string ErpWebhookUrl { get; set; } = "";
    /// <summary>SDD CRM-039 deferred / 048 — optional Authorization header value (e.g. Bearer …).</summary>
    public string ErpWebhookAuthHeader { get; set; } = "";
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
