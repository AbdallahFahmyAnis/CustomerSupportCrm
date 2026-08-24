using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.CreateTicket;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.GetTicket;

/// <summary>SDD CRM-004 / specs/003-ticket-lifecycle.</summary>
public sealed record GetTicketQuery(Guid Id) : IRequest<TicketDetailDto?>;

public sealed class GetTicketHandler(TicketsDb db) : IRequestHandler<GetTicketQuery, TicketDetailDto?>
{
    public Task<TicketDetailDto?> Handle(GetTicketQuery request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        return Task.FromResult(ticket is null ? null : Map.Detail(ticket));
    }
}
