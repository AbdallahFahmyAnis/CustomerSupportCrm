using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Escalate.UpdateEscalationSettings;

public sealed class UpdateEscalationSettingsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/sla/escalation-settings", async (UpdateEscalationSettingsRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new UpdateEscalationSettingsCommand(
                body.EscalateOnFirstResponseBreach,
                body.EscalateOnResolutionBreach,
                body.EscalateUrgentAlways,
                body.AssignToAgentId,
                body.AssignToAgentName));
            return result.Error is null
                ? Results.Ok(result.Settings)
                : Results.BadRequest(new { error = result.Error });
        });
    }
}
