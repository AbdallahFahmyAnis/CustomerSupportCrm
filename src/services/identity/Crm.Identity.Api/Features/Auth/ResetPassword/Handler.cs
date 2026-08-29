using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.ResetPassword;

/// <summary>SDD CRM-046.</summary>
public sealed class ResetPasswordHandler(IdentityDirectory directory)
    : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    public async Task<ResetPasswordResponse> Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Token) ||
            string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return new ResetPasswordResponse("Email, token, and new password are required.");
        }

        if (request.NewPassword.Length < 6)
        {
            return new ResetPasswordResponse("Password must be at least 6 characters.");
        }

        var error = await directory.ResetPasswordWithTokenAsync(
            request.Email,
            request.Token,
            request.NewPassword,
            cancellationToken);

        var user = await directory.FindByEmailAsync(request.Email, cancellationToken);
        await directory.AppendAuditAsync(
            AuditActions.PasswordResetCompleted,
            error is null,
            user?.Id,
            request.Email.Trim(),
            user?.Id,
            user?.Email ?? request.Email.Trim(),
            error,
            cancellationToken);

        return new ResetPasswordResponse(error);
    }
}
