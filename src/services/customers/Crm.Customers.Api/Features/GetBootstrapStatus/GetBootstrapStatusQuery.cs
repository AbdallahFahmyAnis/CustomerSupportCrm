using Crm.BuildingBlocks.Sdd;
using Crm.Contracts.Customers;
using MediatR;

namespace Crm.Customers.Api.Features.GetBootstrapStatus;

/// <summary>SDD 001-platform-foundation / CRM-041 — example vertical-slice query later specs copy.</summary>
public sealed record GetBootstrapStatusQuery : IRequest<BootstrapStatusDto>;

public sealed class GetBootstrapStatusHandler : IRequestHandler<GetBootstrapStatusQuery, BootstrapStatusDto>
{
    public Task<BootstrapStatusDto> Handle(GetBootstrapStatusQuery request, CancellationToken cancellationToken)
        => Task.FromResult(new BootstrapStatusDto(
            "customers",
            "ready",
            SddStories.PlatformFoundation,
            "vertical-slice-cqrs"));
}
