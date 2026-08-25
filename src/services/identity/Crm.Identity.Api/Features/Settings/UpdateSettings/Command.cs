using MediatR;

namespace Crm.Identity.Api.Features.Settings.UpdateSettings;

/// <summary>SDD CRM-037.</summary>
public sealed record UpdateSettingsCommand(
    string OrganizationName,
    string SupportEmail,
    string DefaultCulture,
    int MaxFailedLoginAttempts,
    int LockoutMinutes,
    Guid? ActorId) : IRequest<UpdateSettingsResponse>;
