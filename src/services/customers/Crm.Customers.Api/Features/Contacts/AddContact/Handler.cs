using Crm.Contracts.Customers;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.Contacts.AddContact;

public sealed class AddContactHandler(CustomersDb db) : IRequestHandler<AddContactCommand, AddContactResponse>
{
    public Task<AddContactResponse> Handle(AddContactCommand request, CancellationToken cancellationToken)
    {
        var customer = db.GetCustomer(request.CustomerId);
        if (customer is null)
        {
            return Task.FromResult(new AddContactResponse(null, "Customer not found."));
        }

        try
        {
            var contact = customer.AddContact(request.Type, request.Value, request.IsPrimary);
            db.InsertContact(contact);
            return Task.FromResult(new AddContactResponse(
                new ContactDto(contact.Id.ToString(), contact.Type, contact.Value, contact.IsPrimary, contact.IsActive),
                null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new AddContactResponse(null, ex.Message));
        }
    }
}
