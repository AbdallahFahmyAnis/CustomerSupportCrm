using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Escalate.UpdateEscalationSettings;

/// <summary>SDD CRM-019 — update escalation settings.</summary>
public sealed record UpdateEscalationSettingsCommand(
    bool EscalateOnFirstResponseBreach,
    bool EscalateOnResolutionBreach,
    bool EscalateUrgentAlways,
    string AssignToAgentId,
    string AssignToAgentName) : IRequest<UpdateEscalationSettingsResponse>;

public sealed record UpdateEscalationSettingsResponse(EscalationSettingsDto? Settings, string? Error);
