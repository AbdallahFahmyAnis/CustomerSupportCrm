using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.AssignTicket;

/// <summary>SDD CRM-006 / specs/003-ticket-lifecycle.</summary>
public sealed record AssignTicketCommand(Guid Id, string? AgentId, string? AgentName, string Actor)
    : IRequest<AssignTicketResponse>;
