using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.EscalateTicket;

public sealed class EscalateTicketHandler(TicketsDb db) : IRequestHandler<EscalateTicketCommand, EscalateTicketResponse>
{
    public Task<EscalateTicketResponse> Handle(EscalateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return Task.FromResult(new EscalateTicketResponse(null, "Ticket not found."));
        }

        try
        {
            ticket.Escalate(request.AssignToAgentId, request.AssignToAgentName, request.Actor);
            db.Update(ticket);
            return Task.FromResult(new EscalateTicketResponse(TicketMap.Summary(ticket), null));
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return Task.FromResult(new EscalateTicketResponse(null, ex.Message));
        }
    }
}
