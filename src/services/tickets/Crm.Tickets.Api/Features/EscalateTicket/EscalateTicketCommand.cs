using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.CreateTicket;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.EscalateTicket;

/// <summary>SDD CRM-007 / specs/003-ticket-lifecycle.</summary>
public sealed record EscalateTicketCommand(
    Guid Id,
    string? AssignToAgentId,
    string? AssignToAgentName,
    string Actor) : IRequest<EscalateTicketResult>;

public sealed record EscalateTicketResult(TicketSummaryDto? Ticket, string? Error);

public sealed class EscalateTicketHandler(TicketsDb db) : IRequestHandler<EscalateTicketCommand, EscalateTicketResult>
{
    public Task<EscalateTicketResult> Handle(EscalateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return Task.FromResult(new EscalateTicketResult(null, "Ticket not found."));
        }

        try
        {
            ticket.Escalate(request.AssignToAgentId, request.AssignToAgentName, request.Actor);
            db.Update(ticket);
            return Task.FromResult(new EscalateTicketResult(Map.Summary(ticket), null));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return Task.FromResult(new EscalateTicketResult(null, ex.Message));
        }
    }
}
