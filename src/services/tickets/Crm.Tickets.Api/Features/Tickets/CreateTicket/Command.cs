using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CreateTicket;

/// <summary>SDD CRM-004 / specs/003-ticket-lifecycle.</summary>
public sealed record CreateTicketCommand(
    Guid CustomerId,
    string CustomerName,
    string Subject,
    string? Description,
    string Category,
    string Priority,
    string Actor) : IRequest<CreateTicketResponse>;
