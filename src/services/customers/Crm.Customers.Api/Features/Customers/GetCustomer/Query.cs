using Crm.Contracts.Customers;
using MediatR;

namespace Crm.Customers.Api.Features.Customers.GetCustomer;

/// <summary>SDD CRM-001 / specs/002-customer-profiles.</summary>
public sealed record GetCustomerQuery(Guid Id) : IRequest<CustomerDetailDto?>;
