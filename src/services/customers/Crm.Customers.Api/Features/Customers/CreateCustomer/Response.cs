using Crm.Contracts.Customers;

namespace Crm.Customers.Api.Features.Customers.CreateCustomer;

public sealed record CreateCustomerResponse(CustomerSummaryDto? Customer, DuplicateWarningDto? Duplicate);
