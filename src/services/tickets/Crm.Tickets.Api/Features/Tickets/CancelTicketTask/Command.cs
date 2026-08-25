using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CancelTicketTask;

/// <summary>SDD CRM-014</summary>
public sealed record CancelTicketTaskCommand(Guid TicketId, Guid TaskId)
    : IRequest<CancelTicketTaskResponse>;

public sealed record CancelTicketTaskResponse(TicketTaskDto? Task, string? Error);
