using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Sla.Api.Features.Escalate.GetEscalationSettings;

public sealed class GetEscalationSettingsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sla/escalation-settings", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetEscalationSettingsQuery())));
    }
}
