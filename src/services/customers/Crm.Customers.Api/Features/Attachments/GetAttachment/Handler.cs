using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.Attachments.GetAttachment;

public sealed class GetAttachmentHandler(CustomersDb db) : IRequestHandler<GetAttachmentQuery, GetAttachmentResponse?>
{
    public Task<GetAttachmentResponse?> Handle(GetAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = db.GetAttachment(request.CustomerId, request.AttachmentId);
        if (attachment is null || !File.Exists(attachment.StoragePath))
        {
            return Task.FromResult<GetAttachmentResponse?>(null);
        }

        return Task.FromResult<GetAttachmentResponse?>(
            new GetAttachmentResponse(attachment.FileName, attachment.ContentType, attachment.StoragePath));
    }
}
