using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Features.Settings.GetSettings;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Settings.UpdateSettings;

public sealed class UpdateSettingsHandler(IdentityDirectory directory)
    : IRequestHandler<UpdateSettingsCommand, UpdateSettingsResponse>
{
    public async Task<UpdateSettingsResponse> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
    {
        var validation = UpdateSettingsValidator.Validate(request);
        if (validation is not null)
        {
            return new UpdateSettingsResponse(null, validation);
        }

        var row = await directory.UpdateSettingsAsync(
            request.OrganizationName,
            request.SupportEmail,
            request.DefaultCulture,
            request.MaxFailedLoginAttempts,
            request.LockoutMinutes,
            cancellationToken,
            request.ProductTitle,
            request.PrimaryColor,
            request.LogoUrl,
            request.ErpWebhookUrl,
            request.ErpWebhookAuthHeader);

        string? actorEmail = null;
        if (request.ActorId is { } actorId)
        {
            var actor = await directory.GetUserAsync(actorId, cancellationToken);
            actorEmail = actor?.Email;
        }

        await directory.AppendAuditAsync(
            AuditActions.SettingsUpdated,
            true,
            request.ActorId,
            actorEmail,
            null,
            null,
            $"Org={row.OrganizationName}; lockout={row.MaxFailedLoginAttempts}/{row.LockoutMinutes}m; culture={row.DefaultCulture}",
            cancellationToken);

        return new UpdateSettingsResponse(GetSettingsHandler.ToDto(row), null);
    }
}
