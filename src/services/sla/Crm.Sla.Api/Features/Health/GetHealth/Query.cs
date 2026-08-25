using Crm.Contracts.Health;
using MediatR;

namespace Crm.Sla.Api.Features.Health.GetHealth;

/// <summary>SDD CRM-017 — health probe.</summary>
public sealed record GetHealthQuery : IRequest<ServiceHealthStatus>;
