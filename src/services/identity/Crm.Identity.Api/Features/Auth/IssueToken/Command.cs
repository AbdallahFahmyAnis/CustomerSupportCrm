using MediatR;

namespace Crm.Identity.Api.Features.Auth.IssueToken;

/// <summary>SDD CRM-035 — password login → access + refresh tokens.</summary>
public sealed record IssueTokenCommand(string Email, string Password) : IRequest<IssueTokenResponse>;
