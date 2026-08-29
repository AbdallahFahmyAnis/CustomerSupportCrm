using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetTicketFeedback;

/// <summary>SDD CRM-030 — GET /api/tickets/feedback?ticketNumber=TKT-1001</summary>
public sealed class GetTicketFeedbackEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets/feedback", async (
            string? ticketNumber,
            Guid? ticketId,
            IMediator mediator) =>
        {
            var feedback = await mediator.Send(new GetTicketFeedbackQuery(ticketId, ticketNumber));
            return feedback is null ? Results.NotFound() : Results.Ok(feedback);
        });
    }
}
