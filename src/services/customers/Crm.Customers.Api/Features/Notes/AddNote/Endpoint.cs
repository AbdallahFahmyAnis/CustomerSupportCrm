using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Customers;
using MediatR;

namespace Crm.Customers.Api.Features.Notes.AddNote;

public sealed class AddNoteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customers/{id:guid}/notes", async (Guid id, AddNoteRequest body, HttpContext http, IMediator mediator) =>
        {
            var author = http.Request.Headers["X-Crm-User-Email"].FirstOrDefault()
                ?? http.Request.Headers["X-Crm-User-Id"].FirstOrDefault()
                ?? "Demo Agent";
            var result = await mediator.Send(new AddNoteCommand(id, body.Body, author));
            if (result.Error is not null)
            {
                return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
                    ? Results.NotFound(new { error = result.Error })
                    : Results.BadRequest(new { error = result.Error });
            }

            return Results.Created($"/api/customers/{id}/notes/{result.Note!.Id}", result.Note);
        });
    }
}
