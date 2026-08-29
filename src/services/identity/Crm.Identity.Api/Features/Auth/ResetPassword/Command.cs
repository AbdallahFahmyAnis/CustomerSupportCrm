using MediatR;

namespace Crm.Identity.Api.Features.Auth.ResetPassword;

/// <summary>SDD CRM-046 / specs/001-identity/050-password-reset.</summary>
public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword)
    : IRequest<ResetPasswordResponse>;

public sealed record ResetPasswordResponse(string? Error);
