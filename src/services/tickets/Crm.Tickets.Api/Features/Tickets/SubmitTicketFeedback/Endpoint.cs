using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.SubmitTicketFeedback;

/// <summary>SDD CRM-030 — POST /api/tickets/feedback and POST /api/tickets/{id}/feedback</summary>
public sealed class SubmitTicketFeedbackEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets/feedback", async (SubmitTicketFeedbackRequest body, IMediator mediator) =>
            await Send(mediator, ParseGuid(body.TicketId), body.TicketNumber, body.Rating, body.Comment));

        app.MapPost("/api/tickets/{id:guid}/feedback", async (
            Guid id,
            SubmitTicketFeedbackRequest body,
            IMediator mediator) =>
            await Send(mediator, id, body.TicketNumber, body.Rating, body.Comment));
    }

    private static async Task<IResult> Send(
        IMediator mediator,
        Guid? ticketId,
        string? ticketNumber,
        int rating,
        string? comment)
    {
        var result = await mediator.Send(new SubmitTicketFeedbackCommand(ticketId, ticketNumber, rating, comment));
        if (result.Error is not null)
        {
            if (result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.BadRequest(new { error = result.Error });
        }

        return Results.Created($"/api/tickets/{result.Feedback!.TicketId}", result.Feedback);
    }

    private static Guid? ParseGuid(string? value) =>
        Guid.TryParse(value, out var id) ? id : null;
}
