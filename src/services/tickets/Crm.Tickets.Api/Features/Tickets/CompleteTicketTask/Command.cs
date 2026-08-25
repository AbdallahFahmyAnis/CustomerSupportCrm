using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CompleteTicketTask;

/// <summary>SDD CRM-014</summary>
public sealed record CompleteTicketTaskCommand(Guid TicketId, Guid TaskId)
    : IRequest<CompleteTicketTaskResponse>;

public sealed record CompleteTicketTaskResponse(TicketTaskDto? Task, string? Error);
