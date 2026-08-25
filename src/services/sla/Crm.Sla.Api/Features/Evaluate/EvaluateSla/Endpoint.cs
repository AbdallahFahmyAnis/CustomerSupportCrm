using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Evaluate.EvaluateSla;

public sealed class EvaluateSlaEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/sla/evaluate", async (EvaluateSlaRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new EvaluateSlaQuery(
                body.Priority,
                body.CreatedAt,
                body.FirstResponseAt,
                body.ResolvedAt,
                body.AsOf));
            if (result.Error is null)
            {
                return Results.Ok(result.Evaluation);
            }

            return result.Error.Contains("Unknown", StringComparison.OrdinalIgnoreCase)
                   || result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                ? Results.NotFound(new { error = result.Error })
                : Results.BadRequest(new { error = result.Error });
        });
    }
}
