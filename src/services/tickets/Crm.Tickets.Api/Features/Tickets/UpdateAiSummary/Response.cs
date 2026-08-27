using Crm.Contracts.Tickets;

namespace Crm.Tickets.Api.Features.Tickets.UpdateAiSummary;

public sealed record UpdateAiSummaryResponse(TicketDetailDto? Ticket, string? Error);
