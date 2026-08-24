using MediatR;

namespace Crm.Identity.Api.Features.Auth.RevokeToken;

/// <summary>SDD CRM-035 — revoke refresh and/or access (jti blacklist).</summary>
public sealed record RevokeTokenCommand(string? RefreshToken, string? AccessToken) : IRequest<RevokeTokenResponse>;
