using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Evaluate.EvaluateSla;

/// <summary>SDD CRM-017 — compute due/breach for a ticket snapshot.</summary>
public sealed record EvaluateSlaQuery(
    string Priority,
    DateTimeOffset CreatedAt,
    DateTimeOffset? FirstResponseAt,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset? AsOf) : IRequest<EvaluateSlaResponse>;

public sealed record EvaluateSlaResponse(SlaEvaluationDto? Evaluation, string? Error);
