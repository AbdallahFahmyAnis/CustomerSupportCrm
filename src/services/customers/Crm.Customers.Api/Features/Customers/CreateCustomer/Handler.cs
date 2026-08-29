using Crm.BuildingBlocks.Audit;
using Crm.Contracts.Customers;
using Crm.Customers.Api.Domain;
using Crm.Customers.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Crm.Customers.Api.Features.Customers.CreateCustomer;

/// <summary>SDD CRM-001 / CRM-036 / specs/051.</summary>
public sealed class CreateCustomerHandler(
    CustomersDb db,
    IdentityAuditClient audit,
    IHttpContextAccessor http) : IRequestHandler<CreateCustomerCommand, CreateCustomerResponse>
{
    public async Task<CreateCustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var existing = db.FindIdByUniqueIdentifier(request.UniqueIdentifier);
        if (existing is not null)
        {
            return new CreateCustomerResponse(
                null,
                new DuplicateWarningDto(
                    "A customer with this unique identifier already exists.",
                    existing.Value.ToString()));
        }

        var customer = Customer.Register(
            request.DisplayName,
            request.UniqueIdentifier,
            request.Organization,
            request.Status);
        db.InsertCustomer(customer);
        await audit.WriteAsync(
            AuditServices.Customers,
            "CustomerCreated",
            true,
            AuditActor.Email(http),
            request.UniqueIdentifier,
            $"Created {customer.DisplayName}",
            cancellationToken);
        return new CreateCustomerResponse(
            new CustomerSummaryDto(
                customer.Id.ToString(),
                customer.DisplayName,
                customer.Organization,
                customer.Status,
                customer.UniqueIdentifier),
            null);
    }
}
