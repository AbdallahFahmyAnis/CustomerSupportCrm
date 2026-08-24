using Crm.Contracts.Customers;
using Crm.Customers.Api.Domain;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.AddContact;

/// <summary>SDD CRM-002 / specs/002-customer-profiles.</summary>
public sealed record AddContactCommand(Guid CustomerId, string Type, string Value, bool IsPrimary)
    : IRequest<AddContactResult>;

public sealed record AddContactResult(ContactDto? Contact, string? Error);

public sealed class AddContactHandler(CustomersDb db) : IRequestHandler<AddContactCommand, AddContactResult>
{
    public Task<AddContactResult> Handle(AddContactCommand request, CancellationToken cancellationToken)
    {
        var customer = db.GetCustomer(request.CustomerId);
        if (customer is null)
        {
            return Task.FromResult(new AddContactResult(null, "Customer not found."));
        }

        try
        {
            var contact = customer.AddContact(request.Type, request.Value, request.IsPrimary);
            db.InsertContact(contact);
            return Task.FromResult(new AddContactResult(
                new ContactDto(contact.Id.ToString(), contact.Type, contact.Value, contact.IsPrimary, contact.IsActive),
                null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new AddContactResult(null, ex.Message));
        }
    }
}
