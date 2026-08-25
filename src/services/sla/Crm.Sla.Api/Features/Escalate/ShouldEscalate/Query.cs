using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Escalate.ShouldEscalate;

/// <summary>SDD CRM-019 — decide if a ticket should escalate.</summary>
public sealed record ShouldEscalateQuery(
    string Priority,
    DateTimeOffset CreatedAt,
    bool IsEscalated,
    string? Status,
    string? AssignedAgentId,
    DateTimeOffset? FirstResponseAt,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset? AsOf) : IRequest<ShouldEscalateDto>;
