using Crm.Contracts.Sla;
using Crm.Sla.Api.Domain;
using Crm.Sla.Api.Features.Shared;
using Crm.Sla.Api.Infrastructure;
using MediatR;

namespace Crm.Sla.Api.Features.Assign.SuggestAssignee;

public sealed class SuggestAssigneeHandler(SlaDb db)
    : IRequestHandler<SuggestAssigneeQuery, SuggestAssigneeDto>
{
    public Task<SuggestAssigneeDto> Handle(SuggestAssigneeQuery request, CancellationToken cancellationToken)
    {
        var match = AutoAssignMatcher.Suggest(db.ListAssignRules(), request.Category, request.Priority);
        return Task.FromResult(SlaMap.Suggest(match));
    }
}
