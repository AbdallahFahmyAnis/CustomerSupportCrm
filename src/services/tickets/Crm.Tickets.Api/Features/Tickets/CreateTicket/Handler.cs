using Crm.Tickets.Api.Domain;
using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.CreateTicket;

public sealed class CreateTicketHandler(TicketsDb db) : IRequestHandler<CreateTicketCommand, CreateTicketResponse>
{
    public Task<CreateTicketResponse> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ticket = Ticket.Create(
                db.NextTicketNumber(),
                request.CustomerId,
                request.CustomerName,
                request.Subject,
                request.Description,
                request.Category,
                request.Priority,
                request.Actor);
            db.Insert(ticket);
            return Task.FromResult(new CreateTicketResponse(TicketMap.Summary(ticket), null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new CreateTicketResponse(null, ex.Message));
        }
    }
}
