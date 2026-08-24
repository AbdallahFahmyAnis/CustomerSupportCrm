using Crm.BuildingBlocks.Endpoints;
using MediatR;

namespace Crm.Customers.Api.Features.Attachments.GetAttachment;

public sealed class GetAttachmentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customers/{id:guid}/attachments/{attachmentId:guid}",
            async (Guid id, Guid attachmentId, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetAttachmentQuery(id, attachmentId));
                return result is null
                    ? Results.NotFound()
                    : Results.File(result.StoragePath, result.ContentType, result.FileName);
            });
    }
}
