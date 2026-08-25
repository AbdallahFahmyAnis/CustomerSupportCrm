using Crm.Sla.Api.Domain;
using Crm.Sla.Api.Features.Shared;
using Crm.Sla.Api.Infrastructure;
using MediatR;

namespace Crm.Sla.Api.Features.Assign.ReplaceAssignRules;

public sealed class ReplaceAssignRulesHandler(SlaDb db)
    : IRequestHandler<ReplaceAssignRulesCommand, ReplaceAssignRulesResponse>
{
    public Task<ReplaceAssignRulesResponse> Handle(ReplaceAssignRulesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var rules = request.Rules.Select(r =>
            {
                Guid? id = Guid.TryParse(r.Id, out var parsed) ? parsed : null;
                return AutoAssignRule.Create(r.Category, r.Priority, r.AgentId, r.AgentName, r.Enabled, id);
            }).ToList();

            if (rules.Count == 0)
            {
                return Task.FromResult(new ReplaceAssignRulesResponse(null, "At least one assign rule is required."));
            }

            db.ReplaceAssignRules(rules);
            IReadOnlyList<Crm.Contracts.Sla.AutoAssignRuleDto> dto = db.ListAssignRules().Select(SlaMap.AssignRule).ToList();
            return Task.FromResult(new ReplaceAssignRulesResponse(dto, null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new ReplaceAssignRulesResponse(null, ex.Message));
        }
    }
}
