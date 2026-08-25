using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Escalate.GetEscalationSettings;

/// <summary>SDD CRM-019 — get escalation settings.</summary>
public sealed record GetEscalationSettingsQuery : IRequest<EscalationSettingsDto>;
