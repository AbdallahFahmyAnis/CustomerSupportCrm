using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.GetAttachment;

/// <summary>SDD CRM-003 / specs/002-customer-profiles.</summary>
public sealed record GetAttachmentQuery(Guid CustomerId, Guid AttachmentId) : IRequest<GetAttachmentResult?>;

public sealed record GetAttachmentResult(string FileName, string ContentType, string StoragePath);

public sealed class GetAttachmentHandler(CustomersDb db) : IRequestHandler<GetAttachmentQuery, GetAttachmentResult?>
{
    public Task<GetAttachmentResult?> Handle(GetAttachmentQuery request, CancellationToken cancellationToken)
    {
        var attachment = db.GetAttachment(request.CustomerId, request.AttachmentId);
        if (attachment is null || !File.Exists(attachment.StoragePath))
        {
            return Task.FromResult<GetAttachmentResult?>(null);
        }

        return Task.FromResult<GetAttachmentResult?>(
            new GetAttachmentResult(attachment.FileName, attachment.ContentType, attachment.StoragePath));
    }
}
