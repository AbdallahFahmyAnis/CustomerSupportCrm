using Crm.Contracts.Health;
using MediatR;

namespace Crm.Tickets.Api.Features.Health.GetHealth;

/// <summary>SDD 001-platform-foundation — Tickets liveness.</summary>
public sealed record GetHealthQuery : IRequest<ServiceHealthStatus>;
