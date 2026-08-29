using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Domain;
using Crm.Tickets.Api.Features.Tickets.SubmitTicketFeedback;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetTicketFeedback;

/// <summary>SDD CRM-030</summary>
public sealed class GetTicketFeedbackHandler(TicketsDb db)
    : IRequestHandler<GetTicketFeedbackQuery, TicketFeedbackDto?>
{
    public Task<TicketFeedbackDto?> Handle(
        GetTicketFeedbackQuery request,
        CancellationToken cancellationToken)
    {
        Ticket? ticket = null;
        if (request.TicketId is Guid id && id != Guid.Empty)
        {
            ticket = db.Get(id);
        }
        else if (!string.IsNullOrWhiteSpace(request.TicketNumber))
        {
            ticket = db.GetByTicketNumber(request.TicketNumber);
        }

        if (ticket is null)
        {
            return Task.FromResult<TicketFeedbackDto?>(null);
        }

        var feedback = db.GetFeedback(ticket.Id);
        return Task.FromResult(
            feedback is null ? null : SubmitTicketFeedbackHandler.Map(feedback));
    }
}
