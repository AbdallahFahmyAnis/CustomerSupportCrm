using Crm.Contracts.Customers;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.UpdateCustomer;

/// <summary>SDD CRM-001 / specs/002-customer-profiles.</summary>
public sealed record UpdateCustomerCommand(
    Guid Id,
    string DisplayName,
    string UniqueIdentifier,
    string? Organization,
    string? Status) : IRequest<UpdateCustomerResult>;

public sealed record UpdateCustomerResult(CustomerSummaryDto? Customer, string? Error, DuplicateWarningDto? Duplicate);

public sealed class UpdateCustomerHandler(CustomersDb db) : IRequestHandler<UpdateCustomerCommand, UpdateCustomerResult>
{
    public Task<UpdateCustomerResult> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = db.GetCustomer(request.Id);
        if (customer is null)
        {
            return Task.FromResult(new UpdateCustomerResult(null, "Customer not found.", null));
        }

        var existing = db.FindIdByUniqueIdentifier(request.UniqueIdentifier);
        if (existing is not null && existing.Value != request.Id)
        {
            return Task.FromResult(new UpdateCustomerResult(
                null,
                null,
                new DuplicateWarningDto(
                    "A customer with this unique identifier already exists.",
                    existing.Value.ToString())));
        }

        customer.UpdateProfile(request.DisplayName, request.UniqueIdentifier, request.Organization, request.Status);
        db.UpdateCustomerProfile(customer);
        return Task.FromResult(new UpdateCustomerResult(
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
