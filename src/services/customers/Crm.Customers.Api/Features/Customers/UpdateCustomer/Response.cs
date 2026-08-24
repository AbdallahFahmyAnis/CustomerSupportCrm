using Crm.Contracts.Customers;

namespace Crm.Customers.Api.Features.Customers.UpdateCustomer;

public sealed record UpdateCustomerResponse(
    CustomerSummaryDto? Customer,
    string? Error,
    DuplicateWarningDto? Duplicate);
