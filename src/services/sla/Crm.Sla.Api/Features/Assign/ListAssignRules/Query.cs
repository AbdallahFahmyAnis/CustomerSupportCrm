using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Assign.ListAssignRules;

/// <summary>SDD CRM-018 — list auto-assign rules.</summary>
public sealed record ListAssignRulesQuery : IRequest<IReadOnlyList<AutoAssignRuleDto>>;
