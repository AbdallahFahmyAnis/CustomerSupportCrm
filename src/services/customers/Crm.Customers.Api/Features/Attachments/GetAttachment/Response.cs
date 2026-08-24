namespace Crm.Customers.Api.Features.Attachments.GetAttachment;

public sealed record GetAttachmentResponse(string FileName, string ContentType, string StoragePath);
