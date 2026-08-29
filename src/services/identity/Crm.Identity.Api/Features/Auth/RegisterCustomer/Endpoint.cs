using Crm.BuildingBlocks.Endpoints;
using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.RegisterCustomer;

/// <summary>SDD CRM-045 — anonymous POST /api/identity/register.</summary>
public sealed class RegisterCustomerEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/identity/register", async (RegisterCustomerRequest body, IMediator mediator) =>
        {
            var result = await mediator.Send(new RegisterCustomerCommand(
                body.Email,
                body.DisplayName,
                body.Password));
            if (result.Tokens is not null)
            {
                return Results.Ok(result.Tokens);
            }

            var err = result.Error ?? "Registration failed.";
            var conflict = err.Contains("already exists", StringComparison.OrdinalIgnoreCase);
            return Results.Json(
                new { error = err },
                statusCode: conflict ? StatusCodes.Status409Conflict : StatusCodes.Status400BadRequest);
        });
    }
}
