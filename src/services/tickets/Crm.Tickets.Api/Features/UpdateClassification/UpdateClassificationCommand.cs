using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.CreateTicket;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.UpdateClassification;

/// <summary>SDD CRM-005 / specs/003-ticket-lifecycle.</summary>
public sealed record UpdateClassificationCommand(Guid Id, string Category, string Priority, string Actor)
    : IRequest<UpdateClassificationResult>;

public sealed record UpdateClassificationResult(TicketSummaryDto? Ticket, string? Error);

public sealed class UpdateClassificationHandler(TicketsDb db)
    : IRequestHandler<UpdateClassificationCommand, UpdateClassificationResult>
{
    public Task<UpdateClassificationResult> Handle(UpdateClassificationCommand request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return Task.FromResult(new UpdateClassificationResult(null, "Ticket not found."));
        }

        try
        {
            ticket.Classify(request.Category, request.Priority, request.Actor);
            db.Update(ticket);
            return Task.FromResult(new UpdateClassificationResult(Map.Summary(ticket), null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new UpdateClassificationResult(null, ex.Message));
        }
    }
}
