using Crm.Contracts.Customers;
using Crm.Customers.Api.Domain;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.AddAttachment;

/// <summary>SDD CRM-003 / specs/002-customer-profiles.</summary>
public sealed record AddAttachmentCommand(
    Guid CustomerId,
    string FileName,
    string ContentType,
    Stream Content) : IRequest<AddAttachmentResult>;

public sealed record AddAttachmentResult(AttachmentDto? Attachment, string? Error);

public sealed class AddAttachmentHandler(CustomersDb db) : IRequestHandler<AddAttachmentCommand, AddAttachmentResult>
{
    public async Task<AddAttachmentResult> Handle(AddAttachmentCommand request, CancellationToken cancellationToken)
    {
        var customer = db.GetCustomer(request.CustomerId);
        if (customer is null)
        {
            return new AddAttachmentResult(null, "Customer not found.");
        }

        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            return new AddAttachmentResult(null, "File name is required.");
        }

        var safeName = Path.GetFileName(request.FileName);
        var dir = Path.Combine(db.AttachmentsRoot, request.CustomerId.ToString("N"));
        Directory.CreateDirectory(dir);
        var attachmentId = Guid.NewGuid();
        var storagePath = Path.Combine(dir, $"{attachmentId:N}_{safeName}");

        await using (var file = File.Create(storagePath))
        {
            await request.Content.CopyToAsync(file, cancellationToken);
        }

        var size = new FileInfo(storagePath).Length;
        var attachment = Attachment.Rehydrate(
            attachmentId,
            request.CustomerId,
            safeName,
            string.IsNullOrWhiteSpace(request.ContentType) ? "application/octet-stream" : request.ContentType,
            size,
            storagePath,
            DateTimeOffset.UtcNow);
        db.InsertAttachment(attachment);
        return new AddAttachmentResult(
            new AttachmentDto(attachment.Id.ToString(), attachment.FileName, attachment.ContentType, attachment.SizeBytes, attachment.CreatedAt),
            null);
    }
}
