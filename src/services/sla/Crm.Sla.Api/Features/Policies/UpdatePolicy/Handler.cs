using Crm.Sla.Api.Domain;
using Crm.Sla.Api.Features.Shared;
using Crm.Sla.Api.Infrastructure;
using MediatR;

namespace Crm.Sla.Api.Features.Policies.UpdatePolicy;

public sealed class UpdatePolicyHandler(SlaDb db)
    : IRequestHandler<UpdatePolicyCommand, UpdatePolicyResponse>
{
    public Task<UpdatePolicyResponse> Handle(UpdatePolicyCommand request, CancellationToken cancellationToken)
    {
        if (!SlaCatalog.IsKnownPriority(request.Priority))
        {
            return Task.FromResult(new UpdatePolicyResponse(null, "Unknown priority."));
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
            return Task.FromResult(new UpdatePolicyResponse(SlaMap.Policy(policy), null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new UpdatePolicyResponse(null, ex.Message));
        }
    }
}
