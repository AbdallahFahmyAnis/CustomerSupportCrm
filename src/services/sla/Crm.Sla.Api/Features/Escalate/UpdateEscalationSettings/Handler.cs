using Crm.Sla.Api.Features.Shared;
using Crm.Sla.Api.Infrastructure;
using MediatR;

namespace Crm.Sla.Api.Features.Escalate.UpdateEscalationSettings;

public sealed class UpdateEscalationSettingsHandler(SlaDb db)
    : IRequestHandler<UpdateEscalationSettingsCommand, UpdateEscalationSettingsResponse>
{
    public Task<UpdateEscalationSettingsResponse> Handle(
        UpdateEscalationSettingsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var settings = db.GetEscalationSettings();
            settings.Update(
                request.EscalateOnFirstResponseBreach,
                request.EscalateOnResolutionBreach,
                request.EscalateUrgentAlways,
                request.AssignToAgentId,
                request.AssignToAgentName);
            db.SaveEscalationSettings(settings);
            return Task.FromResult(new UpdateEscalationSettingsResponse(SlaMap.Escalation(settings), null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new UpdateEscalationSettingsResponse(null, ex.Message));
        }
    }
}
