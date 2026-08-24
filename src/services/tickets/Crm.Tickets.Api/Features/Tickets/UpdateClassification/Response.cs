using Crm.Contracts.Tickets;

namespace Crm.Tickets.Api.Features.Tickets.UpdateClassification;

public sealed record UpdateClassificationResponse(TicketSummaryDto? Ticket, string? Error);
