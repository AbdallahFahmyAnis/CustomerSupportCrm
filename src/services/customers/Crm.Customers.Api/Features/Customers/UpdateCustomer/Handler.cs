using Crm.Contracts.Customers;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.Customers.UpdateCustomer;

public sealed class UpdateCustomerHandler(CustomersDb db) : IRequestHandler<UpdateCustomerCommand, UpdateCustomerResponse>
{
    public Task<UpdateCustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = db.GetCustomer(request.Id);
        if (customer is null)
        {
            return Task.FromResult(new UpdateCustomerResponse(null, "Customer not found.", null));
        }

        var existing = db.FindIdByUniqueIdentifier(request.UniqueIdentifier);
        if (existing is not null && existing.Value != request.Id)
        {
            return Task.FromResult(new UpdateCustomerResponse(
                null,
                null,
                new DuplicateWarningDto(
                    "A customer with this unique identifier already exists.",
                    existing.Value.ToString())));
        }

        customer.UpdateProfile(request.DisplayName, request.UniqueIdentifier, request.Organization, request.Status);
        db.UpdateCustomerProfile(customer);
        return Task.FromResult(new UpdateCustomerResponse(
            new CustomerSummaryDto(
                customer.Id.ToString(),
                customer.DisplayName,
                customer.Organization,
                customer.Status,
                customer.UniqueIdentifier),
            null,
            null));
    }
}
