using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ListTicketTasks;

/// <summary>SDD CRM-014</summary>
public sealed class ListTicketTasksHandler(TicketsDb db)
    : IRequestHandler<ListTicketTasksQuery, IReadOnlyList<TicketTaskDto>>
{
    public Task<IReadOnlyList<TicketTaskDto>> Handle(ListTicketTasksQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<TicketTaskDto> items = db.ListTasks(request.TicketId).Select(Map).ToList();
        return Task.FromResult(items);
    }

    internal static TicketTaskDto Map(Domain.TicketTask t) => new(
        t.Id.ToString(),
        t.TicketId.ToString(),
        t.Title,
        t.DueAt,
        t.AssigneeUserId,
        t.AssigneeName,
        t.Status,
        t.CreatedAt,
        t.UpdatedAt);
}
