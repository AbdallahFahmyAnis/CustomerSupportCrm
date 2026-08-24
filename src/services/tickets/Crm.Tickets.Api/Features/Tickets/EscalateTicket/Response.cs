using Crm.Contracts.Tickets;

namespace Crm.Tickets.Api.Features.Tickets.EscalateTicket;

public sealed record EscalateTicketResponse(TicketSummaryDto? Ticket, string? Error);
