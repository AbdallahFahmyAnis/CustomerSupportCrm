using MediatR;

namespace Crm.Customers.Api.Features.Attachments.GetAttachment;

/// <summary>SDD CRM-003 / specs/002-customer-profiles.</summary>
public sealed record GetAttachmentQuery(Guid CustomerId, Guid AttachmentId) : IRequest<GetAttachmentResponse?>;
