using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Customers.Api.Features.Attachments.AddAttachment;

public sealed class AddAttachmentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/customers/{id:guid}/attachments", async (Guid id, HttpRequest request, IMediator mediator) =>
        {
            if (!request.HasFormContentType)
            {
                return Results.BadRequest(new { error = "multipart form required" });
            }

            var form = await request.ReadFormAsync();
            var file = form.Files.GetFile("file") ?? form.Files.FirstOrDefault();
            if (file is null || file.Length == 0)
            {
                return Results.BadRequest(new { error = "file is required" });
            }

            await using var stream = file.OpenReadStream();
            var result = await mediator.Send(new AddAttachmentCommand(id, file.FileName, file.ContentType, stream));
            if (result.Error is not null)
            {
                return Results.NotFound(new { error = result.Error });
            }

            return Results.Created($"/api/customers/{id}/attachments/{result.Attachment!.Id}", result.Attachment);
        });
    }
}
