using Crm.BuildingBlocks.Diagnostics;
using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Features.CreateUser;
using Crm.Identity.Api.Features.DeactivateUser;
using Crm.Identity.Api.Features.DevLogin;
using Crm.Identity.Api.Features.GetHealth;
using Crm.Identity.Api.Features.ListRoles;
using Crm.Identity.Api.Features.SearchUsers;
using Crm.Identity.Api.Features.UpdateUserRole;
using Crm.Identity.Api.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSingleton<IdentityDb>();

var app = builder.Build();
app.UseCorrelationId();

var db = app.Services.GetRequiredService<IdentityDb>();
db.EnsureSchema();
db.SeedIfEmpty();

static bool IsAdmin(HttpContext http) =>
    string.Equals(http.Request.Headers["X-Crm-User-Role"].FirstOrDefault(), RoleNames.Admin, StringComparison.OrdinalIgnoreCase);

static Guid? ActorId(HttpContext http)
{
    var raw = http.Request.Headers["X-Crm-User-Id"].FirstOrDefault();
    return Guid.TryParse(raw, out var id) ? id : null;
}

app.MapGet("/health", async (IMediator mediator) => Results.Ok(await mediator.Send(new GetHealthQuery())));

app.MapPost("/api/identity/dev-login", async (DevLoginCommand command, IMediator mediator) =>
{
    var user = await mediator.Send(command);
    return user is null ? Results.Unauthorized() : Results.Ok(user);
});

app.MapGet("/api/identity/users", async (string? q, IMediator mediator) =>
    Results.Ok(await mediator.Send(new SearchUsersQuery(q))));

app.MapPost("/api/identity/users", async (CreateUserRequest body, HttpContext http, IMediator mediator) =>
{
    if (!IsAdmin(http))
    {
        return Results.Json(new { error = "Admin role required." }, statusCode: StatusCodes.Status403Forbidden);
    }

    var result = await mediator.Send(new CreateUserCommand(body.Email, body.DisplayName, body.Password, body.Role));
    return result.Error is not null
        ? Results.BadRequest(new { error = result.Error })
        : Results.Created($"/api/identity/users/{result.User!.Id}", result.User);
});

app.MapPost("/api/identity/users/{id:guid}/role", async (Guid id, UpdateUserRoleRequest body, HttpContext http, IMediator mediator) =>
{
    if (!IsAdmin(http))
    {
        return Results.Json(new { error = "Admin role required." }, statusCode: StatusCodes.Status403Forbidden);
    }

    var result = await mediator.Send(new UpdateUserRoleCommand(id, body.Role));
    if (result.Error is null)
    {
        return Results.Ok(result.User);
    }

    return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
        ? Results.NotFound(new { error = result.Error })
        : Results.BadRequest(new { error = result.Error });
});

app.MapPost("/api/identity/users/{id:guid}/deactivate", async (Guid id, HttpContext http, IMediator mediator) =>
{
    if (!IsAdmin(http))
    {
        return Results.Json(new { error = "Admin role required." }, statusCode: StatusCodes.Status403Forbidden);
    }

    var result = await mediator.Send(new DeactivateUserCommand(id, ActorId(http)));
    if (result.Error is null)
    {
        return Results.Ok(result.User);
    }

    return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
        ? Results.NotFound(new { error = result.Error })
        : Results.BadRequest(new { error = result.Error });
});

app.MapGet("/api/identity/roles", async (IMediator mediator) =>
    Results.Ok(await mediator.Send(new ListRolesQuery())));

app.MapGet("/api/identity/permissions", async (IMediator mediator) =>
    Results.Ok(await mediator.Send(new GetPermissionCatalogQuery())));

app.Run();

public partial class Program;
