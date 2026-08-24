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
        return Task.FromResult(ticket is null ? null : TicketMap.Detail(ticket));
    }
}
