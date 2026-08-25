using MediatR;

namespace Crm.Identity.Api.Features.Settings.UpdateSettings;

/// <summary>SDD CRM-037 / CRM-044 / CRM-039.</summary>
public sealed record UpdateSettingsCommand(
    string OrganizationName,
    string SupportEmail,
    string DefaultCulture,
    int MaxFailedLoginAttempts,
    int LockoutMinutes,
    Guid? ActorId,
    string? ProductTitle = null,
    string? PrimaryColor = null,
    string? LogoUrl = null,
    string? ErpWebhookUrl = null) : IRequest<UpdateSettingsResponse>;
