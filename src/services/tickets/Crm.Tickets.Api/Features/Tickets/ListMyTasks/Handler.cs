using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.Tickets.ListTicketTasks;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ListMyTasks;

/// <summary>SDD CRM-014</summary>
public sealed class ListMyTasksHandler(TicketsDb db)
    : IRequestHandler<ListMyTasksQuery, IReadOnlyList<TicketTaskDto>>
{
    public Task<IReadOnlyList<TicketTaskDto>> Handle(ListMyTasksQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<TicketTaskDto> items =
            db.ListOpenTasks(request.AssigneeUserId, request.DueOnOrBefore)
                .Select(ListTicketTasksHandler.Map)
                .ToList();
        return Task.FromResult(items);
    }
}
