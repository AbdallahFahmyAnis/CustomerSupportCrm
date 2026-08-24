using MediatR;

namespace Crm.Customers.Api.Features.Contacts.DeactivateContact;

/// <summary>SDD CRM-002 / specs/002-customer-profiles.</summary>
public sealed record DeactivateContactCommand(Guid CustomerId, Guid ContactId) : IRequest<DeactivateContactResponse>;
