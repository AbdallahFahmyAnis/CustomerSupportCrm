using Crm.Contracts.Sla;
using Crm.Sla.Api.Features.Shared;
using Crm.Sla.Api.Infrastructure;
using MediatR;

namespace Crm.Sla.Api.Features.Policies.ListPolicies;

public sealed class ListPoliciesHandler(SlaDb db)
    : IRequestHandler<ListPoliciesQuery, IReadOnlyList<SlaPolicyDto>>
{
    public Task<IReadOnlyList<SlaPolicyDto>> Handle(ListPoliciesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<SlaPolicyDto> items = db.ListPolicies().Select(SlaMap.Policy).ToList();
        return Task.FromResult(items);
    }
}
