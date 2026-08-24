using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetTicketOptions;

public sealed class GetTicketOptionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets/options", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetTicketOptionsQuery())));
    }
}
