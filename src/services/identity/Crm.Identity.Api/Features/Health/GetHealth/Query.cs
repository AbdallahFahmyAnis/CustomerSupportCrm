using Crm.Contracts.Health;
using MediatR;

namespace Crm.Identity.Api.Features.Health.GetHealth;

/// <summary>SDD 001-platform-foundation — Identity liveness query.</summary>
public sealed record GetHealthQuery : IRequest<ServiceHealthStatus>;
