using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.EscalateTicket;

/// <summary>SDD CRM-007 / specs/003-ticket-lifecycle.</summary>
public sealed record EscalateTicketCommand(
    Guid Id,
    string? AssignToAgentId,
    string? AssignToAgentName,
    string Actor) : IRequest<EscalateTicketResponse>;
