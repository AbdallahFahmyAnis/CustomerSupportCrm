using Crm.Tickets.Api.Features.Tickets.ListTicketTasks;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CompleteTicketTask;

/// <summary>SDD CRM-014</summary>
public sealed class CompleteTicketTaskHandler(TicketsDb db)
    : IRequestHandler<CompleteTicketTaskCommand, CompleteTicketTaskResponse>
{
    public Task<CompleteTicketTaskResponse> Handle(CompleteTicketTaskCommand request, CancellationToken cancellationToken)
    {
        var task = db.GetTask(request.TaskId);
        if (task is null || task.TicketId != request.TicketId)
        {
            return Task.FromResult(new CompleteTicketTaskResponse(null, "Task not found."));
        }

        try
        {
            task.Complete();
            db.UpdateTask(task);
            return Task.FromResult(new CompleteTicketTaskResponse(ListTicketTasksHandler.Map(task), null));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(new CompleteTicketTaskResponse(null, ex.Message));
        }
    }
}
