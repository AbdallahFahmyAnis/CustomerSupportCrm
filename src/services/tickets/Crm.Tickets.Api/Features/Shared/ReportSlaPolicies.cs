using Crm.Tickets.Api.Domain;

namespace Crm.Tickets.Api.Features.Shared;

/// <summary>SDD CRM-032 — demo SLA resolution minutes (mirrors SLA seed).</summary>
internal static class ReportSlaPolicies
{
    private static readonly Dictionary<string, int> ResolutionMinutes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Low"] = 2880,
        ["Medium"] = 1440,
        ["High"] = 480,
        ["Urgent"] = 240
    };

    public static bool IsResolutionBreached(
        string priority,
        DateTimeOffset createdAt,
        DateTimeOffset? resolvedAt,
        DateTimeOffset asOf)
    {
        var minutes = ResolutionMinutes.GetValueOrDefault(priority, 1440);
        var due = createdAt.AddMinutes(minutes);
        var clock = resolvedAt ?? asOf;
        return clock > due;
    }

    public static DateTimeOffset? ResolvedAt(string status, DateTimeOffset updatedAt) =>
        status.Equals(TicketStatuses.Resolved, StringComparison.OrdinalIgnoreCase) ||
        status.Equals(TicketStatuses.Closed, StringComparison.OrdinalIgnoreCase)
            ? updatedAt
            : null;
}
