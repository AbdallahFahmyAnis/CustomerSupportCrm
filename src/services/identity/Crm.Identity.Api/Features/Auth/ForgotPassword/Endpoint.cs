using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.ForgotPassword;

/// <summary>SDD CRM-046 — anonymous POST /api/identity/forgot-password.</summary>
public sealed class ForgotPasswordEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/forgot-password", async (ForgotPasswordRequest body, IMediator mediator) =>
            Results.Ok(await mediator.Send(new ForgotPasswordCommand(body.Email))));
    }
}
