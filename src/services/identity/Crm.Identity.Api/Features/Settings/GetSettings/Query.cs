using MediatR;

namespace Crm.Identity.Api.Features.Settings.GetSettings;

/// <summary>SDD CRM-037.</summary>
public sealed record GetSettingsQuery : IRequest<GetSettingsResponse>;
