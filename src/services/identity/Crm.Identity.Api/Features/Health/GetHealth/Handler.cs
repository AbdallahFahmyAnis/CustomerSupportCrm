using Crm.Contracts.Health;
using MediatR;

namespace Crm.Identity.Api.Features.Health.GetHealth;

public sealed class GetHealthHandler : IRequestHandler<GetHealthQuery, ServiceHealthStatus>
{
    public Task<ServiceHealthStatus> Handle(GetHealthQuery request, CancellationToken cancellationToken)
        => Task.FromResult(new ServiceHealthStatus("identity", "ok"));
}
