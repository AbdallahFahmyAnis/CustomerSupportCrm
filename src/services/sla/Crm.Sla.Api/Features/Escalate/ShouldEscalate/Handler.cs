using Crm.Contracts.Sla;
using Crm.Sla.Api.Domain;
using Crm.Sla.Api.Infrastructure;
using MediatR;

namespace Crm.Sla.Api.Features.Escalate.ShouldEscalate;

public sealed class ShouldEscalateHandler(SlaDb db)
    : IRequestHandler<ShouldEscalateQuery, ShouldEscalateDto>
{
    public Task<ShouldEscalateDto> Handle(ShouldEscalateQuery request, CancellationToken cancellationToken)
    {
        var settings = db.GetEscalationSettings();
        var asOf = request.AsOf ?? DateTimeOffset.UtcNow;

        DateTimeOffset? firstResponseAt = request.FirstResponseAt;
        if (firstResponseAt is null &&
            (!string.IsNullOrWhiteSpace(request.AssignedAgentId) ||
             (request.Status is not null &&
              !request.Status.Equals("New", StringComparison.OrdinalIgnoreCase))))
        {
            // Treat assigned / moved out of New as first response met for automation.
            firstResponseAt = request.CreatedAt;
        }

        DateTimeOffset? resolvedAt = request.ResolvedAt;
        if (resolvedAt is null &&
            request.Status is not null &&
            (request.Status.Equals("Resolved", StringComparison.OrdinalIgnoreCase) ||
             request.Status.Equals("Closed", StringComparison.OrdinalIgnoreCase)))
        {
            resolvedAt = asOf;
        }

        SlaEvaluation? evaluation = null;
        var policy = db.GetPolicy(request.Priority);
        if (policy is not null)
        {
            evaluation = SlaClock.Evaluate(policy, request.CreatedAt, firstResponseAt, resolvedAt, asOf);
        }

        var (should, reason) = EscalationDecision.Decide(settings, request.Priority, request.IsEscalated, evaluation);
        if (!should)
        {
            return Task.FromResult(new ShouldEscalateDto(false, null, null, null));
        }

        return Task.FromResult(new ShouldEscalateDto(
            true,
            settings.AssignToAgentId,
            settings.AssignToAgentName,
            reason));
    }
}
