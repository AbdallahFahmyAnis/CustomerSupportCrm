using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.DeactivateContact;

/// <summary>SDD CRM-002 / specs/002-customer-profiles.</summary>
public sealed record DeactivateContactCommand(Guid CustomerId, Guid ContactId) : IRequest<DeactivateContactResult>;

public sealed record DeactivateContactResult(bool Ok, string? Error);

public sealed class DeactivateContactHandler(CustomersDb db)
    : IRequestHandler<DeactivateContactCommand, DeactivateContactResult>
{
    public Task<DeactivateContactResult> Handle(DeactivateContactCommand request, CancellationToken cancellationToken)
    {
        try
        {
            db.DeactivateContact(request.CustomerId, request.ContactId);
            return Task.FromResult(new DeactivateContactResult(true, null));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(new DeactivateContactResult(false, ex.Message));
        }
    }
}
