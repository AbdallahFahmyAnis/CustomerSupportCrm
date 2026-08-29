using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.ForgotPassword;

/// <summary>SDD CRM-046 — always generic success; optional dev token + Channels email.</summary>
public sealed class ForgotPasswordHandler(
    IdentityDirectory directory,
    IConfiguration config,
    IHostEnvironment env,
    IHttpClientFactory httpFactory,
    ILogger<ForgotPasswordHandler> log)
    : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
{
    private const string GenericMessage =
        "If an account exists for that email, a reset link has been sent.";

    public async Task<ForgotPasswordResponse> Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return new ForgotPasswordResponse(GenericMessage);
        }

        var email = request.Email.Trim();
        var token = await directory.CreatePasswordResetTokenAsync(email, cancellationToken);
        string? devToken = null;

        if (token is not null)
        {
            var user = await directory.FindByEmailAsync(email, cancellationToken);
            await directory.AppendAuditAsync(
                AuditActions.PasswordResetRequested,
                true,
                user?.Id,
                email,
                user?.Id,
                email,
                null,
                cancellationToken);

            var expose = config.GetValue("Identity:ExposeResetToken", env.IsDevelopment());
            if (expose)
            {
                devToken = token;
            }

            var publicOrigin = config["PublicOrigin"] ?? "http://localhost:5000";
            var link =
                $"{publicOrigin.TrimEnd('/')}/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

            await TrySendEmailAsync(email, link, cancellationToken);

            if (expose)
            {
                log.LogInformation("CRM-046 password reset link for {Email}: {Link}", email, link);
            }
        }
        else
        {
            await directory.AppendAuditAsync(
                AuditActions.PasswordResetRequested,
                false,
                null,
                email,
                null,
                email,
                "Unknown or inactive",
                cancellationToken);
        }

        return new ForgotPasswordResponse(GenericMessage, devToken);
    }

    private async Task TrySendEmailAsync(string to, string link, CancellationToken ct)
    {
        try
        {
            var channels = config["Services:Channels"] ?? "http://localhost:5201";
            var client = httpFactory.CreateClient("downstream");
            using var response = await client.PostAsJsonAsync(
                $"{channels.TrimEnd('/')}/api/channels/mail/send",
                new
                {
                    to,
                    subject = "Reset your CRM password",
                    body =
                        $"Use this link to reset your password (expires soon):\n\n{link}\n\nIf you did not request this, ignore this email."
                },
                ct);
            if (!response.IsSuccessStatusCode)
            {
                log.LogWarning("CRM-046 Channels mail send returned {Status}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "CRM-046 Channels mail send skipped");
        }
    }
}
