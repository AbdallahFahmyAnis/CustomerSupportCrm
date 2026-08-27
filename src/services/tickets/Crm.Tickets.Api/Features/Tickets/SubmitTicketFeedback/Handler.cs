using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Domain;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.SubmitTicketFeedback;

/// <summary>SDD CRM-030</summary>
public sealed class SubmitTicketFeedbackHandler(TicketsDb db)
    : IRequestHandler<SubmitTicketFeedbackCommand, SubmitTicketFeedbackResponse>
{
    public Task<SubmitTicketFeedbackResponse> Handle(
        SubmitTicketFeedbackCommand request,
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
            return Task.FromResult(new SubmitTicketFeedbackResponse(null, "Ticket not found."));
        }

        if (!ticket.Status.Equals(TicketStatuses.Resolved, StringComparison.OrdinalIgnoreCase) &&
            !ticket.Status.Equals(TicketStatuses.Closed, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new SubmitTicketFeedbackResponse(
                null,
                "Feedback is only allowed for Resolved or Closed tickets."));
        }

        if (db.GetFeedback(ticket.Id) is not null)
        {
            return Task.FromResult(new SubmitTicketFeedbackResponse(
                null,
                "Feedback already exists for this ticket."));
        }

        try
        {
            var feedback = TicketFeedback.Create(ticket.Id, request.Rating, request.Comment);
            db.InsertFeedback(feedback);
            return Task.FromResult(new SubmitTicketFeedbackResponse(Map(feedback), null));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return Task.FromResult(new SubmitTicketFeedbackResponse(null, ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(new SubmitTicketFeedbackResponse(null, ex.Message));
        }
    }

    internal static TicketFeedbackDto Map(TicketFeedback f) => new(
        f.Id.ToString(),
        f.TicketId.ToString(),
        f.Rating,
        f.Comment,
        f.CreatedAt);
}
