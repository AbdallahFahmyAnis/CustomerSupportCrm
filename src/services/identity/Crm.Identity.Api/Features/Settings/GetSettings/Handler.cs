using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Settings.GetSettings;

public sealed class GetSettingsHandler(IdentityDirectory directory)
    : IRequestHandler<GetSettingsQuery, GetSettingsResponse>
{
    public async Task<GetSettingsResponse> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var row = await directory.GetOrCreateSettingsAsync(cancellationToken);
        return new GetSettingsResponse(ToDto(row));
    }

    internal static SystemSettingsDto ToDto(Domain.SystemSettings row) =>
        new(
            row.OrganizationName,
            row.SupportEmail,
            row.DefaultCulture,
            row.MaxFailedLoginAttempts,
            row.LockoutMinutes,
            row.UpdatedAt);
}
