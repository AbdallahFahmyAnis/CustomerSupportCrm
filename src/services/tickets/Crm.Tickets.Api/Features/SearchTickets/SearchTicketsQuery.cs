using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.CreateTicket;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.SearchTickets;

/// <summary>SDD CRM-004 / specs/003-ticket-lifecycle.</summary>
public sealed record SearchTicketsQuery(string? Q, string? AssignedAgentId)
    : IRequest<IReadOnlyList<TicketSummaryDto>>;

public sealed class SearchTicketsHandler(TicketsDb db)
    : IRequestHandler<SearchTicketsQuery, IReadOnlyList<TicketSummaryDto>>
{
    public Task<IReadOnlyList<TicketSummaryDto>> Handle(SearchTicketsQuery request, CancellationToken cancellationToken)
    {
        var rows = db.Search(request.Q, request.AssignedAgentId).Select(Map.Summary).ToList();
        return Task.FromResult<IReadOnlyList<TicketSummaryDto>>(rows);
    }
}
