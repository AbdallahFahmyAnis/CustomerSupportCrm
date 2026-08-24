using Crm.Contracts.Health;
using MediatR;

namespace Crm.Customers.Api.Features.Health.GetHealth;

/// <summary>SDD 001-platform-foundation — Customers liveness query.</summary>
public sealed record GetHealthQuery : IRequest<ServiceHealthStatus>;
