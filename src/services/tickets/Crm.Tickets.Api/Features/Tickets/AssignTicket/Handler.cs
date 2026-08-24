using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.AssignTicket;

public sealed class AssignTicketHandler(TicketsDb db) : IRequestHandler<AssignTicketCommand, AssignTicketResponse>
{
    public Task<AssignTicketResponse> Handle(AssignTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return Task.FromResult(new AssignTicketResponse(null, "Ticket not found."));
        }

        try
        {
            ticket.Assign(request.AgentId, request.AgentName, request.Actor);
            db.Update(ticket);
            return Task.FromResult(new AssignTicketResponse(TicketMap.Summary(ticket), null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new AssignTicketResponse(null, ex.Message));
        }
    }
}
