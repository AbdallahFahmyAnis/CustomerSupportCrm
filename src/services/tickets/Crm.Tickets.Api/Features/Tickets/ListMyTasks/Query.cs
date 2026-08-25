using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ListMyTasks;

/// <summary>SDD CRM-014 — open tasks for assignee / due filter.</summary>
public sealed record ListMyTasksQuery(string? AssigneeUserId, DateTimeOffset? DueOnOrBefore)
    : IRequest<IReadOnlyList<TicketTaskDto>>;
