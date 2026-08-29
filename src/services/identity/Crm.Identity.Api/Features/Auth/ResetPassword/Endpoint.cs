using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.ResetPassword;

/// <summary>SDD CRM-046 — anonymous POST /api/identity/reset-password.</summary>
public sealed class ResetPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/reset-password", async (ResetPasswordRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new ResetPasswordCommand(
                body.Email,
                body.Token,
                body.NewPassword));
            return result.Error is null
                ? Results.NoContent()
                : Results.BadRequest(new { error = result.Error });
        });
    }
}
