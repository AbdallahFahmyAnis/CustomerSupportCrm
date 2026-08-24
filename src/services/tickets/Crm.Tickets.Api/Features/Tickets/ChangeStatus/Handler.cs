using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ChangeStatus;

public sealed class ChangeStatusHandler(TicketsDb db) : IRequestHandler<ChangeStatusCommand, ChangeStatusResponse>
{
    public Task<ChangeStatusResponse> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return Task.FromResult(new ChangeStatusResponse(null, "Ticket not found."));
        }

        try
        {
            ticket.ChangeStatus(request.Status, request.Actor);
            db.Update(ticket);
            return Task.FromResult(new ChangeStatusResponse(TicketMap.Summary(ticket), null));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(new ChangeStatusResponse(null, ex.Message));
        }
    }
}
