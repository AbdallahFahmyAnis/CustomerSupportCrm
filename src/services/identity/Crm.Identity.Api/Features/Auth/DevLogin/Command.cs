using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.DevLogin;

/// <summary>SDD CRM-035 — backward-compatible login alias → token issuance.</summary>
public sealed record DevLoginCommand(string Email, string Password) : IRequest<TokenResponseDto?>;
