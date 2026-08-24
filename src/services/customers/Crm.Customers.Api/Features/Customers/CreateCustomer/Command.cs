using MediatR;

namespace Crm.Customers.Api.Features.Customers.CreateCustomer;

/// <summary>SDD CRM-001 / specs/002-customer-profiles.</summary>
public sealed record CreateCustomerCommand(
    string DisplayName,
    string UniqueIdentifier,
    string? Organization,
    string? Status) : IRequest<CreateCustomerResponse>;
