using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Assign.ReplaceAssignRules;

/// <summary>SDD CRM-018 — replace all auto-assign rules.</summary>
public sealed record ReplaceAssignRulesCommand(IReadOnlyList<AutoAssignRuleDto> Rules)
    : IRequest<ReplaceAssignRulesResponse>;

public sealed record ReplaceAssignRulesResponse(IReadOnlyList<AutoAssignRuleDto>? Rules, string? Error);
