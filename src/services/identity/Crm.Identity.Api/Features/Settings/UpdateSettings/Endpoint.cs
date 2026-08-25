using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Features.Shared;
using MediatR;

namespace Crm.Identity.Api.Features.Settings.UpdateSettings;

/// <summary>SDD CRM-037 — update system settings.</summary>
public sealed class UpdateSettingsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/identity/settings", async (UpdateSystemSettingsRequest body, HttpContext http, IMediator mediator) =>
        {
            if (!AdminHttp.IsAdmin(http))
            {
                return AdminHttp.ForbiddenAdmin();
            }

            var result = await mediator.Send(new UpdateSettingsCommand(
                body.OrganizationName,
                body.SupportEmail,
                body.DefaultCulture,
                body.MaxFailedLoginAttempts,
                body.LockoutMinutes,
                AdminHttp.ActorId(http),
                body.ProductTitle,
                body.PrimaryColor,
                body.LogoUrl,
                body.ErpWebhookUrl));

            return result.Error is not null
                ? Results.BadRequest(new { error = result.Error })
                : Results.Ok(result.Settings);
        });
    }
}
