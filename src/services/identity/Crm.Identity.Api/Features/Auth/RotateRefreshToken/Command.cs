using Crm.Identity.Api.Features.Auth.IssueToken;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.RotateRefreshToken;

/// <summary>SDD CRM-035 — rotate refresh token and issue new access token.</summary>
public sealed record RotateRefreshTokenCommand(string RefreshToken) : IRequest<IssueTokenResponse>;
