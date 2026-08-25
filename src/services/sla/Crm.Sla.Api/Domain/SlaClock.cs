namespace Crm.Sla.Api.Domain;

/// <summary>SDD CRM-017 — pure due/breach evaluation.</summary>
public static class SlaClock
{
    public static SlaEvaluation Evaluate(
        SlaPolicy policy,
        DateTimeOffset createdAt,
        DateTimeOffset? firstResponseAt,
        DateTimeOffset? resolvedAt,
        DateTimeOffset asOf)
    {
        var firstDue = createdAt.AddMinutes(policy.FirstResponseMinutes);
        var resolutionDue = createdAt.AddMinutes(policy.ResolutionMinutes);
        var firstBreached = firstResponseAt is null && asOf > firstDue;
        var resolutionBreached = resolvedAt is null && asOf > resolutionDue;
        return new SlaEvaluation(
            policy.Priority,
            policy.FirstResponseMinutes,
            policy.ResolutionMinutes,
            firstDue,
            resolutionDue,
            firstBreached,
            resolutionBreached,
            asOf);
    }
}

/// <summary>SDD CRM-017 — evaluation result (domain).</summary>
public sealed record SlaEvaluation(
    string Priority,
    int FirstResponseMinutes,
    int ResolutionMinutes,
    DateTimeOffset FirstResponseDueAt,
    DateTimeOffset ResolutionDueAt,
    bool FirstResponseBreached,
    bool ResolutionBreached,
    DateTimeOffset AsOf);
