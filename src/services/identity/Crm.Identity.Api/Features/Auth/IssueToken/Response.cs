using Crm.Contracts.Identity;

namespace Crm.Identity.Api.Features.Auth.IssueToken;

public sealed record IssueTokenResponse(TokenResponseDto? Tokens, string? Error, bool Locked);
