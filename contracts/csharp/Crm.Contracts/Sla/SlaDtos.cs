namespace Crm.Contracts.Sla;

/// <summary>SDD CRM-017 — SLA policy row by ticket priority.</summary>
public sealed record SlaPolicyDto(
    string Priority,
    int FirstResponseMinutes,
    int ResolutionMinutes,
    DateTimeOffset UpdatedAt);

/// <summary>SDD CRM-017 — update policy minutes.</summary>
public sealed record UpdateSlaPolicyRequest(int FirstResponseMinutes, int ResolutionMinutes);

/// <summary>SDD CRM-017 — evaluate due clocks for a ticket snapshot.</summary>
public sealed record EvaluateSlaRequest(
    string Priority,
    DateTimeOffset CreatedAt,
    DateTimeOffset? FirstResponseAt = null,
    DateTimeOffset? ResolvedAt = null,
    DateTimeOffset? AsOf = null);

/// <summary>SDD CRM-017 — computed due times and breach flags.</summary>
public sealed record SlaEvaluationDto(
    string Priority,
    int FirstResponseMinutes,
    int ResolutionMinutes,
    DateTimeOffset FirstResponseDueAt,
    DateTimeOffset ResolutionDueAt,
    bool FirstResponseBreached,
    bool ResolutionBreached,
    DateTimeOffset AsOf);

/// <summary>SDD CRM-018 — auto-assign rule (null category/priority = wildcard).</summary>
public sealed record AutoAssignRuleDto(
    string Id,
    string? Category,
    string? Priority,
    string AgentId,
    string AgentName,
    bool Enabled);

public sealed record ReplaceAutoAssignRulesRequest(IReadOnlyList<AutoAssignRuleDto> Rules);

public sealed record SuggestAssigneeRequest(string Category, string Priority);

public sealed record SuggestAssigneeDto(string? AgentId, string? AgentName, string? MatchedRuleId);

/// <summary>SDD CRM-019 — escalation settings singleton.</summary>
public sealed record EscalationSettingsDto(
    bool EscalateOnFirstResponseBreach,
    bool EscalateOnResolutionBreach,
    bool EscalateUrgentAlways,
    string AssignToAgentId,
    string AssignToAgentName,
    DateTimeOffset UpdatedAt);

public sealed record UpdateEscalationSettingsRequest(
    bool EscalateOnFirstResponseBreach,
    bool EscalateOnResolutionBreach,
    bool EscalateUrgentAlways,
    string AssignToAgentId,
    string AssignToAgentName);

public sealed record ShouldEscalateRequest(
    string Priority,
    DateTimeOffset CreatedAt,
    bool IsEscalated,
    string? Status = null,
    string? AssignedAgentId = null,
    DateTimeOffset? FirstResponseAt = null,
    DateTimeOffset? ResolvedAt = null,
    DateTimeOffset? AsOf = null);

public sealed record ShouldEscalateDto(
    bool ShouldEscalate,
    string? AssignToAgentId,
    string? AssignToAgentName,
    string? Reason);
