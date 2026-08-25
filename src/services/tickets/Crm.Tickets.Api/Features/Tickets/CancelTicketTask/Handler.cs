using Crm.Tickets.Api.Features.Tickets.ListTicketTasks;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CancelTicketTask;

/// <summary>SDD CRM-014</summary>
public sealed class CancelTicketTaskHandler(TicketsDb db)
    : IRequestHandler<CancelTicketTaskCommand, CancelTicketTaskResponse>
{
    public Task<CancelTicketTaskResponse> Handle(CancelTicketTaskCommand request, CancellationToken cancellationToken)
    {
        var task = db.GetTask(request.TaskId);
        if (task is null || task.TicketId != request.TicketId)
        {
            return Task.FromResult(new CancelTicketTaskResponse(null, "Task not found."));
        }

        try
        {
            task.Cancel();
            db.UpdateTask(task);
            return Task.FromResult(new CancelTicketTaskResponse(ListTicketTasksHandler.Map(task), null));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(new CancelTicketTaskResponse(null, ex.Message));
        }
    }
}
