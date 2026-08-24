using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.UpdateClassification;

public sealed class UpdateClassificationHandler(TicketsDb db)
    : IRequestHandler<UpdateClassificationCommand, UpdateClassificationResponse>
{
    public Task<UpdateClassificationResponse> Handle(UpdateClassificationCommand request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return Task.FromResult(new UpdateClassificationResponse(null, "Ticket not found."));
        }

        try
        {
            ticket.Classify(request.Category, request.Priority, request.Actor);
            db.Update(ticket);
            return Task.FromResult(new UpdateClassificationResponse(TicketMap.Summary(ticket), null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new UpdateClassificationResponse(null, ex.Message));
        }
    }
}
