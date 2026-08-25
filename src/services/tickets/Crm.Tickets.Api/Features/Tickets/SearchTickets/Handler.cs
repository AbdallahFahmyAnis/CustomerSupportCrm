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
        Guid? dept = null;
        if (!string.IsNullOrWhiteSpace(request.DepartmentId) && Guid.TryParse(request.DepartmentId, out var d))
        {
            dept = d;
        }

        var rows = db.Search(request.Q, request.AssignedAgentId, dept).Select(TicketMap.Summary).ToList();
        return Task.FromResult<IReadOnlyList<TicketSummaryDto>>(rows);
    }
}
