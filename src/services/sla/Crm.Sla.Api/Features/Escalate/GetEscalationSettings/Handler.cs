using Crm.Contracts.Sla;
using Crm.Sla.Api.Features.Shared;
using Crm.Sla.Api.Infrastructure;
using MediatR;

namespace Crm.Sla.Api.Features.Escalate.GetEscalationSettings;

public sealed class GetEscalationSettingsHandler(SlaDb db)
    : IRequestHandler<GetEscalationSettingsQuery, EscalationSettingsDto>
{
    public Task<EscalationSettingsDto> Handle(GetEscalationSettingsQuery request, CancellationToken cancellationToken)
        => Task.FromResult(SlaMap.Escalation(db.GetEscalationSettings()));
}
