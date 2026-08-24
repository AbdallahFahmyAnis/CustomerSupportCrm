using Crm.Contracts.Customers;
using MediatR;

namespace Crm.Customers.Api.Features.Bootstrap.GetBootstrapStatus;

/// <summary>SDD 001-platform-foundation / CRM-041.</summary>
public sealed record GetBootstrapStatusQuery : IRequest<BootstrapStatusDto>;
