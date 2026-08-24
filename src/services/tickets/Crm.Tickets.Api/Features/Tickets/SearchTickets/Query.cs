using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.SearchTickets;

/// <summary>SDD CRM-004 / specs/003-ticket-lifecycle.</summary>
public sealed record SearchTicketsQuery(string? Q, string? AssignedAgentId)
    : IRequest<IReadOnlyList<TicketSummaryDto>>;
