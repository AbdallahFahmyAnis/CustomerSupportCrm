using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ListTicketTasks;

/// <summary>SDD CRM-014</summary>
public sealed record ListTicketTasksQuery(Guid TicketId) : IRequest<IReadOnlyList<TicketTaskDto>>;
