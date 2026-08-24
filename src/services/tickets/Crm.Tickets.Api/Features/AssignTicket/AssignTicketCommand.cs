using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.CreateTicket;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.AssignTicket;

/// <summary>SDD CRM-006 / specs/003-ticket-lifecycle.</summary>
public sealed record AssignTicketCommand(Guid Id, string? AgentId, string? AgentName, string Actor)
    : IRequest<AssignTicketResult>;

public sealed record AssignTicketResult(TicketSummaryDto? Ticket, string? Error);

public sealed class AssignTicketHandler(TicketsDb db) : IRequestHandler<AssignTicketCommand, AssignTicketResult>
{
    public Task<AssignTicketResult> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return Task.FromResult(new AssignTicketResult(null, "Ticket not found."));
        }

        try
        {
            ticket.Assign(request.AgentId, request.AgentName, request.Actor);
            db.Update(ticket);
            return Task.FromResult(new AssignTicketResult(Map.Summary(ticket), null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new AssignTicketResult(null, ex.Message));
        }
    }
}
