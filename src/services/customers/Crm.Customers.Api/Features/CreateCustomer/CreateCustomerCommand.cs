using Crm.Contracts.Customers;
using Crm.Customers.Api.Domain;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.CreateCustomer;

/// <summary>SDD CRM-001 / specs/002-customer-profiles.</summary>
public sealed record CreateCustomerCommand(
    string DisplayName,
    string UniqueIdentifier,
    string? Organization,
    string? Status) : IRequest<CreateCustomerResult>;

public sealed record CreateCustomerResult(CustomerSummaryDto? Customer, DuplicateWarningDto? Duplicate);

public sealed class CreateCustomerHandler(CustomersDb db) : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    public Task<CreateCustomerResult> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var existing = db.FindIdByUniqueIdentifier(request.UniqueIdentifier);
        if (existing is not null)
        {
            return Task.FromResult(new CreateCustomerResult(
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
        return Task.FromResult(new CreateCustomerResult(
            new CustomerSummaryDto(
                customer.Id.ToString(),
                customer.DisplayName,
                customer.Organization,
                customer.Status,
                customer.UniqueIdentifier),
            null));
    }
}
