namespace Crm.Sla.Api.Domain;

/// <summary>SDD CRM-019 — escalation settings.</summary>
public sealed class EscalationSettings
{
    public const string SingletonId = "default";

    public bool EscalateOnFirstResponseBreach { get; private set; }
    public bool EscalateOnResolutionBreach { get; private set; }
    public bool EscalateUrgentAlways { get; private set; }
    public string AssignToAgentId { get; private set; } = "";
    public string AssignToAgentName { get; private set; } = "";
    public DateTimeOffset UpdatedAt { get; private set; }

    private EscalationSettings()
    {
    }

    public static EscalationSettings CreateDefault() => new()
    {
        EscalateOnFirstResponseBreach = true,
        EscalateOnResolutionBreach = true,
        EscalateUrgentAlways = true,
        AssignToAgentId = "22222222-2222-2222-2222-222222222222",
        AssignToAgentName = "Lead Agent",
        UpdatedAt = DateTimeOffset.UtcNow
    };

    public static EscalationSettings Rehydrate(
        bool escalateOnFirstResponseBreach,
        bool escalateOnResolutionBreach,
        bool escalateUrgentAlways,
        string assignToAgentId,
        string assignToAgentName,
        DateTimeOffset updatedAt) => new()
    {
        EscalateOnFirstResponseBreach = escalateOnFirstResponseBreach,
        EscalateOnResolutionBreach = escalateOnResolutionBreach,
        EscalateUrgentAlways = escalateUrgentAlways,
        AssignToAgentId = assignToAgentId,
        AssignToAgentName = assignToAgentName,
        UpdatedAt = updatedAt
    };

    public void Update(
        bool escalateOnFirstResponseBreach,
        bool escalateOnResolutionBreach,
        bool escalateUrgentAlways,
        string assignToAgentId,
        string assignToAgentName)
    {
        if (string.IsNullOrWhiteSpace(assignToAgentId) || string.IsNullOrWhiteSpace(assignToAgentName))
        {
            throw new ArgumentException("Escalation target agent is required.");
        }

        EscalateOnFirstResponseBreach = escalateOnFirstResponseBreach;
        EscalateOnResolutionBreach = escalateOnResolutionBreach;
        EscalateUrgentAlways = escalateUrgentAlways;
        AssignToAgentId = assignToAgentId.Trim();
        AssignToAgentName = assignToAgentName.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

/// <summary>SDD CRM-019 — decide whether to escalate a ticket snapshot.</summary>
public static class EscalationDecision
{
    public static (bool Should, string? Reason) Decide(
        EscalationSettings settings,
        string priority,
        bool isEscalated,
        SlaEvaluation? evaluation)
    {
        if (isEscalated)
        {
            return (false, null);
        }

        if (settings.EscalateUrgentAlways &&
            priority.Equals("Urgent", StringComparison.OrdinalIgnoreCase))
        {
            return (true, "Urgent priority rule");
        }

        if (evaluation is not null &&
            settings.EscalateOnFirstResponseBreach &&
            evaluation.FirstResponseBreached)
        {
            return (true, "First response SLA breached");
        }

        if (evaluation is not null &&
            settings.EscalateOnResolutionBreach &&
            evaluation.ResolutionBreached)
        {
            return (true, "Resolution SLA breached");
        }

        return (false, null);
    }
}
