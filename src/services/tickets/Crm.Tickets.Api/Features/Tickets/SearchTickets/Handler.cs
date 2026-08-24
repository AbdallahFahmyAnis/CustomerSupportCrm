using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.SearchTickets;

public sealed class SearchTicketsHandler(TicketsDb db)
    : IRequestHandler<SearchTicketsQuery, IReadOnlyList<TicketSummaryDto>>
{
    public Task<IReadOnlyList<TicketSummaryDto>> Handle(SearchTicketsQuery request, CancellationToken cancellationToken)
    {
        var rows = db.Search(request.Q, request.AssignedAgentId).Select(TicketMap.Summary).ToList();
        return Task.FromResult<IReadOnlyList<TicketSummaryDto>>(rows);
    }
}
