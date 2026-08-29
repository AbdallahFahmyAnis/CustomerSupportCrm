using Crm.BuildingBlocks.Audit;
using Crm.Sla.Api.Features.Shared;
using Crm.Sla.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Crm.Sla.Api.Features.Escalate.UpdateEscalationSettings;

/// <summary>SDD CRM-019 / CRM-036 / specs/051.</summary>
public sealed class UpdateEscalationSettingsHandler(
    SlaDb db,
    IdentityAuditClient audit,
    IHttpContextAccessor http)
    : IRequestHandler<UpdateEscalationSettingsCommand, UpdateEscalationSettingsResponse>
{
    public async Task<UpdateEscalationSettingsResponse> Handle(
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
            await audit.WriteAsync(
                AuditServices.Sla,
                "SlaEscalationUpdated",
                true,
                AuditActor.Email(http),
                request.AssignToAgentName,
                "Escalation settings saved",
                cancellationToken);
            return new UpdateEscalationSettingsResponse(SlaMap.Escalation(settings), null);
        }
        catch (ArgumentException ex)
        {
            return new UpdateEscalationSettingsResponse(null, ex.Message);
        }
    }
}
