using Crm.BuildingBlocks.Sdd;
using Crm.Contracts.Customers;
using MediatR;

namespace Crm.Customers.Api.Features.Bootstrap.GetBootstrapStatus;

public sealed class GetBootstrapStatusHandler : IRequestHandler<GetBootstrapStatusQuery, BootstrapStatusDto>
{
    public Task<BootstrapStatusDto> Handle(GetBootstrapStatusQuery request, CancellationToken cancellationToken)
        => Task.FromResult(new BootstrapStatusDto(
            "customers",
            "ready",
            SddStories.PlatformFoundation,
            "vertical-slice-cqrs"));
}
