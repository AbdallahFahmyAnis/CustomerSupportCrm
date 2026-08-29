using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetTicketFeedback;

/// <summary>SDD CRM-030 — read CSAT for a ticket (portal).</summary>
public sealed record GetTicketFeedbackQuery(Guid? TicketId, string? TicketNumber)
    : IRequest<TicketFeedbackDto?>;
