using Crm.Contracts.Tickets;

namespace Crm.Tickets.Api.Features.Tickets.ChangeStatus;

public sealed record ChangeStatusResponse(TicketSummaryDto? Ticket, string? Error);
