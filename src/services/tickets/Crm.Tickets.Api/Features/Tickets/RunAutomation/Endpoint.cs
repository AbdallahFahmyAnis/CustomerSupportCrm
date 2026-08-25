using Crm.BuildingBlocks.Endpoints;
using Crm.Tickets.Api.Features.Shared;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.RunAutomation;

public sealed class RunAutomationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tickets/{id:guid}/run-automation",
            async (Guid id, HttpContext http, IMediator mediator) =>
            {
                var result = await mediator.Send(new RunAutomationCommand(id, TicketHttp.Actor(http)));
                if (result.Error is null)
                {
                    return Results.Ok(result.Result);
                }

                return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? Results.NotFound(new { error = result.Error })
                    : Results.BadRequest(new { error = result.Error });
            });
    }
}
