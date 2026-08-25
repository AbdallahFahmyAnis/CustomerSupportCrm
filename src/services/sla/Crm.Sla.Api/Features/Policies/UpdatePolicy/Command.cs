using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Policies.UpdatePolicy;

/// <summary>SDD CRM-017 — update one priority policy.</summary>
public sealed record UpdatePolicyCommand(string Priority, int FirstResponseMinutes, int ResolutionMinutes)
    : IRequest<UpdatePolicyResponse>;

public sealed record UpdatePolicyResponse(SlaPolicyDto? Policy, string? Error);
