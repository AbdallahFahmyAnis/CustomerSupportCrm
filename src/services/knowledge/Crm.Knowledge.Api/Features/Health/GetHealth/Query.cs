using Crm.Contracts.Health;
using MediatR;

namespace Crm.Knowledge.Api.Features.Health.GetHealth;

/// <summary>SDD CRM-021 — health probe.</summary>
public sealed record GetHealthQuery : IRequest<ServiceHealthStatus>;
