using Crm.Contracts.Sla;
using Crm.Sla.Api.Domain;

namespace Crm.Sla.Api.Features.Shared;

/// <summary>SDD CRM-017 — map domain ↔ contracts.</summary>
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
}
