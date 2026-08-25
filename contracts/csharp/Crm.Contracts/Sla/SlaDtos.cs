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
