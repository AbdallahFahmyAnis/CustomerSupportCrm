using Crm.Sla.Api.Domain;
using Crm.Sla.Api.Features.Shared;
using Crm.Sla.Api.Infrastructure;
using MediatR;

namespace Crm.Sla.Api.Features.Evaluate.EvaluateSla;

public sealed class EvaluateSlaHandler(SlaDb db)
    : IRequestHandler<EvaluateSlaQuery, EvaluateSlaResponse>
{
    public Task<EvaluateSlaResponse> Handle(EvaluateSlaQuery request, CancellationToken cancellationToken)
    {
        if (!SlaCatalog.IsKnownPriority(request.Priority))
        {
            return Task.FromResult(new EvaluateSlaResponse(null, "Unknown priority."));
        }

        var policy = db.GetPolicy(request.Priority);
        if (policy is null)
        {
            return Task.FromResult(new EvaluateSlaResponse(null, "Policy not found."));
        }

        var asOf = request.AsOf ?? DateTimeOffset.UtcNow;
        var evaluation = SlaClock.Evaluate(
            policy,
            request.CreatedAt,
            request.FirstResponseAt,
            request.ResolvedAt,
            asOf);
        return Task.FromResult(new EvaluateSlaResponse(SlaMap.Evaluation(evaluation), null));
    }
}
