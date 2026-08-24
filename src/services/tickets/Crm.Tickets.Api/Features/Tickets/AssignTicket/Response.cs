using Crm.Contracts.Tickets;

namespace Crm.Tickets.Api.Features.Tickets.AssignTicket;

public sealed record AssignTicketResponse(TicketSummaryDto? Ticket, string? Error);
