using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.RunAutomation;

/// <summary>SDD CRM-018 / CRM-019 — apply SLA assign + escalate suggestions.</summary>
public sealed record RunAutomationCommand(Guid Id, string Actor) : IRequest<RunAutomationResponse>;

public sealed record RunAutomationResponse(RunAutomationResultDto? Result, string? Error);
