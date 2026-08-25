using Crm.Contracts.Sla;
using Crm.Sla.Api.Features.Shared;
using Crm.Sla.Api.Infrastructure;
using MediatR;

namespace Crm.Sla.Api.Features.Assign.ListAssignRules;

public sealed class ListAssignRulesHandler(SlaDb db)
    : IRequestHandler<ListAssignRulesQuery, IReadOnlyList<AutoAssignRuleDto>>
{
    public Task<IReadOnlyList<AutoAssignRuleDto>> Handle(ListAssignRulesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<AutoAssignRuleDto> items = db.ListAssignRules().Select(SlaMap.AssignRule).ToList();
        return Task.FromResult(items);
    }
}
