using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Policies.ListPolicies;

/// <summary>SDD CRM-017 — list SLA policies.</summary>
public sealed record ListPoliciesQuery : IRequest<IReadOnlyList<SlaPolicyDto>>;
