using Crm.Contracts.Health;
using MediatR;

namespace Crm.Tickets.Api.Features.GetHealth;

/// <summary>SDD 001-platform-foundation — Tickets liveness stub until CRM-004.</summary>
public sealed record GetHealthQuery : IRequest<ServiceHealthStatus>;

public sealed class GetHealthHandler : IRequestHandler<GetHealthQuery, ServiceHealthStatus>
{
    public Task<ServiceHealthStatus> Handle(GetHealthQuery request, CancellationToken cancellationToken)
        => Task.FromResult(new ServiceHealthStatus("tickets", "ok"));
}
