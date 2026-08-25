using Crm.Contracts.Sla;
using Crm.Sla.Api.Domain;

namespace Crm.Sla.Api.Features.Shared;

/// <summary>SDD CRM-017 / CRM-018 / CRM-019 — map domain ↔ contracts.</summary>
public static class SlaMap
{
    public static SlaPolicyDto Policy(SlaPolicy policy) => new(
        policy.Priority,
        policy.FirstResponseMinutes,
        policy.ResolutionMinutes,
        policy.UpdatedAt);

    public static SlaEvaluationDto Evaluation(SlaEvaluation evaluation) => new(
        evaluation.Priority,
        evaluation.FirstResponseMinutes,
        evaluation.ResolutionMinutes,
        evaluation.FirstResponseDueAt,
        evaluation.ResolutionDueAt,
        evaluation.FirstResponseBreached,
        evaluation.ResolutionBreached,
        evaluation.AsOf);

    public static SuggestAssigneeDto Suggest(AutoAssignRule? rule) =>
        rule is null
            ? new SuggestAssigneeDto(null, null, null)
            : new SuggestAssigneeDto(rule.AgentId, rule.AgentName, rule.Id.ToString());

    public static AutoAssignRuleDto AssignRule(AutoAssignRule rule) => new(
        rule.Id.ToString(),
        rule.Category,
        rule.Priority,
        rule.AgentId,
        rule.AgentName,
        rule.Enabled);

    public static EscalationSettingsDto Escalation(EscalationSettings settings) => new(
        settings.EscalateOnFirstResponseBreach,
        settings.EscalateOnResolutionBreach,
        settings.EscalateUrgentAlways,
        settings.AssignToAgentId,
        settings.AssignToAgentName,
        settings.UpdatedAt);
}
