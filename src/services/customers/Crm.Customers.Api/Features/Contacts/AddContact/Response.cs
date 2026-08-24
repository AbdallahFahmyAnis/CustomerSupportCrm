using Crm.Contracts.Customers;

namespace Crm.Customers.Api.Features.Contacts.AddContact;

public sealed record AddContactResponse(ContactDto? Contact, string? Error);
