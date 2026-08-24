using Crm.Contracts.Tickets;

namespace Crm.Tickets.Api.Features.Tickets.CreateTicket;

public sealed record CreateTicketResponse(TicketSummaryDto? Ticket, string? Error);
