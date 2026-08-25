using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetTicket;

public sealed class GetTicketHandler(TicketsDb db) : IRequestHandler<GetTicketQuery, TicketDetailDto?>
{
    public Task<TicketDetailDto?> Handle(GetTicketQuery request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return Task.FromResult<TicketDetailDto?>(null);
        }

        var notes = db.ListNotes(request.Id);
        return Task.FromResult<TicketDetailDto?>(TicketMap.Detail(ticket, notes));
    }
}
