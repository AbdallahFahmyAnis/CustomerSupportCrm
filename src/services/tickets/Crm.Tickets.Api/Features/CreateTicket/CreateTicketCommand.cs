using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Domain;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.CreateTicket;

/// <summary>SDD CRM-004 / specs/003-ticket-lifecycle.</summary>
public sealed record CreateTicketCommand(
    Guid CustomerId,
    string CustomerName,
    string Subject,
    string? Description,
    string Category,
    string Priority,
    string Actor) : IRequest<CreateTicketResult>;

public sealed record CreateTicketResult(TicketSummaryDto? Ticket, string? Error);

public sealed class CreateTicketHandler(TicketsDb db) : IRequestHandler<CreateTicketCommand, CreateTicketResult>
{
    public Task<CreateTicketResult> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ticket = Ticket.Create(
                db.NextTicketNumber(),
                request.CustomerId,
                request.CustomerName,
                request.Subject,
                request.Description,
                request.Category,
                request.Priority,
                request.Actor);
            db.Insert(ticket);
            return Task.FromResult(new CreateTicketResult(Map.Summary(ticket), null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new CreateTicketResult(null, ex.Message));
        }
    }
}

internal static class Map
{
    public static TicketSummaryDto Summary(Ticket t) => new(
        t.Id.ToString(),
        t.TicketNumber,
        t.CustomerId.ToString(),
        t.CustomerName,
        t.Subject,
        t.Category,
        t.Priority,
        t.Status,
        t.AssignedAgentId,
        t.AssignedAgentName,
        t.IsEscalated);

    public static TicketDetailDto Detail(Ticket t) => new(
        t.Id.ToString(),
        t.TicketNumber,
        t.CustomerId.ToString(),
        t.CustomerName,
        t.Subject,
        t.Description,
        t.Category,
        t.Priority,
        t.Status,
        t.AssignedAgentId,
        t.AssignedAgentName,
        t.IsEscalated,
        t.CreatedAt,
        t.UpdatedAt,
        t.History
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new TicketHistoryDto(
                h.Id.ToString(),
                h.Field,
                h.OldValue,
                h.NewValue,
                h.ChangedBy,
                h.ChangedAt))
            .ToList());
}
