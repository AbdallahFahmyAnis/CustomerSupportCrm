using Crm.Contracts.Customers;
using Crm.Customers.Api.Domain;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.Customers.CreateCustomer;

public sealed class CreateCustomerHandler(CustomersDb db) : IRequestHandler<CreateCustomerCommand, CreateCustomerResponse>
{
    public Task<CreateCustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var existing = db.FindIdByUniqueIdentifier(request.UniqueIdentifier);
        if (existing is not null)
        {
            return Task.FromResult(new CreateCustomerResponse(
                null,
                new DuplicateWarningDto(
                    "A customer with this unique identifier already exists.",
                    existing.Value.ToString())));
        }

        var customer = Customer.Register(
            request.DisplayName,
            request.UniqueIdentifier,
            request.Organization,
            request.Status);
        db.InsertCustomer(customer);
        return Task.FromResult(new CreateCustomerResponse(
            new CustomerSummaryDto(
                customer.Id.ToString(),
                customer.DisplayName,
                customer.Organization,
                customer.Status,
                customer.UniqueIdentifier),
            null));
    }
}
