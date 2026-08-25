namespace Crm.Sla.Api.Domain;

/// <summary>SDD CRM-017 — priority catalog aligned with Tickets.</summary>
public static class SlaCatalog
{
    public static readonly string[] Priorities = ["Low", "Medium", "High", "Urgent"];

    public static bool IsKnownPriority(string? priority) =>
        !string.IsNullOrWhiteSpace(priority)
        && Priorities.Contains(priority.Trim(), StringComparer.OrdinalIgnoreCase);

    public static string NormalizePriority(string priority) =>
        Priorities.First(p => p.Equals(priority.Trim(), StringComparison.OrdinalIgnoreCase));
}
