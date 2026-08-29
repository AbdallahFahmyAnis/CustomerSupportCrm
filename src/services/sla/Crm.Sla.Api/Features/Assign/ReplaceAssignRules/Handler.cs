using Crm.BuildingBlocks.Audit;
using Crm.Sla.Api.Domain;
using Crm.Sla.Api.Features.Shared;
using Crm.Sla.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Crm.Sla.Api.Features.Assign.ReplaceAssignRules;

/// <summary>SDD CRM-018 / CRM-036 / specs/051.</summary>
public sealed class ReplaceAssignRulesHandler(
    SlaDb db,
    IdentityAuditClient audit,
    IHttpContextAccessor http)
    : IRequestHandler<ReplaceAssignRulesCommand, ReplaceAssignRulesResponse>
{
    public async Task<ReplaceAssignRulesResponse> Handle(ReplaceAssignRulesCommand request, CancellationToken cancellationToken)
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
                return new ReplaceAssignRulesResponse(null, "At least one assign rule is required.");
            }

            db.ReplaceAssignRules(rules);
            IReadOnlyList<Crm.Contracts.Sla.AutoAssignRuleDto> dto = db.ListAssignRules().Select(SlaMap.AssignRule).ToList();
            await audit.WriteAsync(
                AuditServices.Sla,
                "SlaRulesUpdated",
                true,
                AuditActor.Email(http),
                null,
                $"{rules.Count} rule(s)",
                cancellationToken);
            return new ReplaceAssignRulesResponse(dto, null);
        }
        catch (ArgumentException ex)
        {
            return new ReplaceAssignRulesResponse(null, ex.Message);
        }
    }
}
