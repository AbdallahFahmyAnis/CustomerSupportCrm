namespace Crm.Contracts.Identity;

public sealed record DevLoginRequest(string Email, string Password);

public sealed record DevUserDto(string Id, string Email, string Name, string Role);

/// <summary>SDD CRM-035 — access + refresh token pair for gateway BFF.</summary>
public sealed record TokenResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt,
    DevUserDto User);

public sealed record RefreshTokenRequest(string RefreshToken);

public sealed record RevokeTokenRequest(string? RefreshToken, string? AccessToken);
