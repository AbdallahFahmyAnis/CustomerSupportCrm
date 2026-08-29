using Crm.BuildingBlocks.Audit;
using Crm.Sla.Api.Domain;
using Crm.Sla.Api.Features.Shared;
using Crm.Sla.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Crm.Sla.Api.Features.Policies.UpdatePolicy;

/// <summary>SDD CRM-017 / CRM-036 / specs/051.</summary>
public sealed class UpdatePolicyHandler(
    SlaDb db,
    IdentityAuditClient audit,
    IHttpContextAccessor http)
    : IRequestHandler<UpdatePolicyCommand, UpdatePolicyResponse>
{
    public async Task<UpdatePolicyResponse> Handle(UpdatePolicyCommand request, CancellationToken cancellationToken)
    {
        if (!SlaCatalog.IsKnownPriority(request.Priority))
        {
            return new UpdatePolicyResponse(null, "Unknown priority.");
        }

        try
        {
            var existing = db.GetPolicy(request.Priority);
            var policy = existing ?? SlaPolicy.Create(request.Priority, request.FirstResponseMinutes, request.ResolutionMinutes);
            if (existing is not null)
            {
                policy.Update(request.FirstResponseMinutes, request.ResolutionMinutes);
            }

            db.Upsert(policy);
            await audit.WriteAsync(
                AuditServices.Sla,
                "SlaPolicyUpdated",
                true,
                AuditActor.Email(http),
                request.Priority,
                $"FR={request.FirstResponseMinutes}m RES={request.ResolutionMinutes}m",
                cancellationToken);
            return new UpdatePolicyResponse(SlaMap.Policy(policy), null);
        }
        catch (ArgumentException ex)
        {
            return new UpdatePolicyResponse(null, ex.Message);
        }
    }
}
