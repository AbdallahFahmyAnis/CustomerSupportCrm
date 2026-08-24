namespace Crm.Identity.Api.Features.Auth.RotateRefreshToken;

public static class RotateRefreshTokenValidator
{
    public static string? Validate(RotateRefreshTokenCommand command) =>
        string.IsNullOrWhiteSpace(command.RefreshToken) ? "Refresh token is required." : null;
}
