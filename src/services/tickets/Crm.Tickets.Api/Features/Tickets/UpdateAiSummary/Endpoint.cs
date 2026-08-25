using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.UpdateAiSummary;

/// <summary>SDD CRM-023 polish / specs/042</summary>
public sealed class UpdateAiSummaryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/tickets/{id:guid}/ai-summary",
            async (Guid id, UpdateAiSummaryRequest body, IMediator mediator) =>
            {
                var result = await mediator.Send(
                    new UpdateAiSummaryCommand(id, body.Summary, body.Highlights));
                if (result.Error is null)
                {
                    return Results.Ok(result.Ticket);
                }

                return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? Results.NotFound(new { error = result.Error })
                    : Results.BadRequest(new { error = result.Error });
            });
    }
}
