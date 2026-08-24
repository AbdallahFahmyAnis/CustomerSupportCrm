using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.CreateTicket;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.ChangeStatus;

/// <summary>SDD CRM-007 / specs/003-ticket-lifecycle.</summary>
public sealed record ChangeStatusCommand(Guid Id, string Status, string Actor) : IRequest<ChangeStatusResult>;

public sealed record ChangeStatusResult(TicketSummaryDto? Ticket, string? Error);

public sealed class ChangeStatusHandler(TicketsDb db) : IRequestHandler<ChangeStatusCommand, ChangeStatusResult>
{
    public Task<ChangeStatusResult> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return Task.FromResult(new ChangeStatusResult(null, "Ticket not found."));
        }

        try
        {
            ticket.ChangeStatus(request.Status, request.Actor);
            db.Update(ticket);
            return Task.FromResult(new ChangeStatusResult(Map.Summary(ticket), null));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(new ChangeStatusResult(null, ex.Message));
        }
    }
}
