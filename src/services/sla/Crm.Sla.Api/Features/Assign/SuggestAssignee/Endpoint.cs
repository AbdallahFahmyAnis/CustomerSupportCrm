using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Assign.SuggestAssignee;

public sealed class SuggestAssigneeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/sla/suggest-assignee", async (SuggestAssigneeRequest body, IMediator mediator) =>
            Results.Ok(await mediator.Send(new SuggestAssigneeQuery(body.Category, body.Priority))));
    }
}
