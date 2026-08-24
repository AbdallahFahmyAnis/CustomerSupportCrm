using Crm.Contracts.Customers;

namespace Crm.Customers.Api.Features.Attachments.AddAttachment;

public sealed record AddAttachmentResponse(AttachmentDto? Attachment, string? Error);
