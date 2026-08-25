using Crm.Contracts.Identity;

namespace Crm.Identity.Api.Features.Settings.UpdateSettings;

public sealed record UpdateSettingsResponse(SystemSettingsDto? Settings, string? Error);
