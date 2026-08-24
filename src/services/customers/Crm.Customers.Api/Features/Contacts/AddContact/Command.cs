using MediatR;

namespace Crm.Customers.Api.Features.Contacts.AddContact;

/// <summary>SDD CRM-002 / specs/002-customer-profiles.</summary>
public sealed record AddContactCommand(Guid CustomerId, string Type, string Value, bool IsPrimary)
    : IRequest<AddContactResponse>;
