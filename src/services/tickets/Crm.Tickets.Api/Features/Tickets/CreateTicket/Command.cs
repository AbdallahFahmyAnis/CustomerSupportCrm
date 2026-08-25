using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CreateTicket;

/// <summary>SDD CRM-004 / CRM-043.</summary>
public sealed record CreateTicketCommand(
    Guid CustomerId,
    string CustomerName,
    string Subject,
    string? Description,
    string Category,
    string Priority,
    string Actor,
    Guid? DepartmentId = null) : IRequest<CreateTicketResponse>;
