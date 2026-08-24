using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.Contacts.DeactivateContact;

public sealed class DeactivateContactHandler(CustomersDb db)
    : IRequestHandler<DeactivateContactCommand, DeactivateContactResponse>
{
    public Task<DeactivateContactResponse> Handle(DeactivateContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            db.DeactivateContact(request.CustomerId, request.ContactId);
            return Task.FromResult(new DeactivateContactResponse(true, null));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(new DeactivateContactResponse(false, ex.Message));
        }
    }
}
