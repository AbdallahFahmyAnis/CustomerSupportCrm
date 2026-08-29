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

/// <summary>SDD CRM-045 / specs/identity/049-customer-register — public customer self-registration.</summary>
public sealed record RegisterCustomerRequest(string Email, string DisplayName, string Password);

/// <summary>SDD CRM-046 / specs/identity/050-password-reset.</summary>
public sealed record ForgotPasswordRequest(string Email);

public sealed record ForgotPasswordResponse(string Message, string? DevResetToken = null);

public sealed record ResetPasswordRequest(string Token, string Email, string NewPassword);
