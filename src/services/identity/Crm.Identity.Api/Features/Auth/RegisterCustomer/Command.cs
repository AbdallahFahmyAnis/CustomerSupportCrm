using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.RegisterCustomer;

/// <summary>SDD CRM-045 / specs/001-identity/049-customer-register.</summary>
public sealed record RegisterCustomerCommand(
    string Email,
    string DisplayName,
    string Password) : IRequest<RegisterCustomerResponse>;

public sealed record RegisterCustomerResponse(TokenResponseDto? Tokens, string? Error);
