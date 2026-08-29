using Crm.BuildingBlocks.Audit;
using Crm.Contracts.Customers;
using Crm.Customers.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Crm.Customers.Api.Features.Customers.UpdateCustomer;

/// <summary>SDD CRM-001 / CRM-036 / specs/051.</summary>
public sealed class UpdateCustomerHandler(
    CustomersDb db,
    IdentityAuditClient audit,
    IHttpContextAccessor http) : IRequestHandler<UpdateCustomerCommand, UpdateCustomerResponse>
{
    public async Task<UpdateCustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = db.GetCustomer(request.Id);
        if (customer is null)
        {
            return new UpdateCustomerResponse(null, "Customer not found.", null);
        }

        var existing = db.FindIdByUniqueIdentifier(request.UniqueIdentifier);
        if (existing is not null && existing.Value != request.Id)
        {
            return new UpdateCustomerResponse(
                null,
                null,
                new DuplicateWarningDto(
                    "A customer with this unique identifier already exists.",
                    existing.Value.ToString()));
        }

        customer.UpdateProfile(request.DisplayName, request.UniqueIdentifier, request.Organization, request.Status);
        db.UpdateCustomerProfile(customer);
        await audit.WriteAsync(
            AuditServices.Customers,
            "CustomerUpdated",
            true,
            AuditActor.Email(http),
            request.UniqueIdentifier,
            $"Updated {customer.DisplayName}",
            cancellationToken);
        return new UpdateCustomerResponse(
            new CustomerSummaryDto(
                customer.Id.ToString(),
                customer.DisplayName,
                customer.Organization,
                customer.Status,
                customer.UniqueIdentifier),
            null,
            null);
    }
}
