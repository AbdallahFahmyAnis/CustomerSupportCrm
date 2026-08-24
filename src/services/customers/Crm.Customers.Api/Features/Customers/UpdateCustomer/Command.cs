using MediatR;

namespace Crm.Customers.Api.Features.Customers.UpdateCustomer;

/// <summary>SDD CRM-001 / specs/002-customer-profiles.</summary>
public sealed record UpdateCustomerCommand(
    Guid Id,
    string DisplayName,
    string UniqueIdentifier,
    string? Organization,
    string? Status) : IRequest<UpdateCustomerResponse>;
