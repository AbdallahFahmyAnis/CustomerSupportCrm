using Crm.Tickets.Api.Domain;
using Crm.Tickets.Api.Features.Tickets.ListTicketTasks;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CreateTicketTask;

/// <summary>SDD CRM-014</summary>
public sealed class CreateTicketTaskHandler(TicketsDb db)
    : IRequestHandler<CreateTicketTaskCommand, CreateTicketTaskResponse>
{
    public Task<CreateTicketTaskResponse> Handle(CreateTicketTaskCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Task.FromResult(new CreateTicketTaskResponse(null, "Title is required."));
        }

        if (db.Get(request.TicketId) is null)
        {
            return Task.FromResult(new CreateTicketTaskResponse(null, "Ticket not found."));
        }

        var task = TicketTask.Create(
            request.TicketId,
            request.Title,
            request.DueAt,
            request.AssigneeUserId,
            request.AssigneeName);
        db.InsertTask(task);
        return Task.FromResult(new CreateTicketTaskResponse(ListTicketTasksHandler.Map(task), null));
    }
}
