using System.Text.Json;
using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.UpdateAiSummary;

/// <summary>SDD CRM-023 polish / specs/042</summary>
public sealed class UpdateAiSummaryHandler(TicketsDb db)
    : IRequestHandler<UpdateAiSummaryCommand, UpdateAiSummaryResponse>
{
    public Task<UpdateAiSummaryResponse> Handle(UpdateAiSummaryCommand request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return Task.FromResult(new UpdateAiSummaryResponse(null, "Ticket not found."));
        }

        try
        {
            var highlightsJson = request.Highlights is { Count: > 0 }
                ? JsonSerializer.Serialize(request.Highlights)
                : null;
            ticket.SetAiSummary(request.Summary, highlightsJson);
            db.Update(ticket);
            return Task.FromResult(new UpdateAiSummaryResponse(TicketMap.Detail(ticket), null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new UpdateAiSummaryResponse(null, ex.Message));
        }
    }
}
