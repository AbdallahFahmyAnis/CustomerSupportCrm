using MediatR;

namespace Crm.Customers.Api.Features.Attachments.AddAttachment;

/// <summary>SDD CRM-003 / specs/002-customer-profiles.</summary>
public sealed record AddAttachmentCommand(
    Guid CustomerId,
    string FileName,
    string ContentType,
    Stream Content) : IRequest<AddAttachmentResponse>;
