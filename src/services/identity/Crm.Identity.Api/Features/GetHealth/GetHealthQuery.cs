using Crm.Contracts.Health;
using MediatR;

namespace Crm.Identity.Api.Features.GetHealth;

/// <summary>SDD 001-platform-foundation — Identity liveness query.</summary>
public sealed record GetHealthQuery : IRequest<ServiceHealthStatus>;

public sealed class GetHealthHandler : IRequestHandler<GetHealthQuery, ServiceHealthStatus>
{
    public Task<ServiceHealthStatus> Handle(GetHealthQuery request, CancellationToken cancellationToken)
        => Task.FromResult(new ServiceHealthStatus("identity", "ok"));
}
