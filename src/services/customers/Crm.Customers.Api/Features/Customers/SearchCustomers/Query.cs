using Crm.Contracts.Customers;
using MediatR;

namespace Crm.Customers.Api.Features.Customers.SearchCustomers;

/// <summary>SDD CRM-001 / specs/002-customer-profiles.</summary>
public sealed record SearchCustomersQuery(string? Q) : IRequest<IReadOnlyList<CustomerSummaryDto>>;
