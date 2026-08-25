using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CreateTicketTask;

/// <summary>SDD CRM-014</summary>
public sealed record CreateTicketTaskCommand(
    Guid TicketId,
    string Title,
    DateTimeOffset? DueAt,
    string? AssigneeUserId,
    string? AssigneeName) : IRequest<CreateTicketTaskResponse>;

public sealed record CreateTicketTaskResponse(TicketTaskDto? Task, string? Error);
