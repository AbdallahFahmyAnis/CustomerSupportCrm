using Crm.BuildingBlocks.Diagnostics;
using Crm.Identity.Api.Features.DevLogin;
using Crm.Identity.Api.Features.GetHealth;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();
app.UseCorrelationId();

app.MapGet("/health", async (IMediator mediator) => Results.Ok(await mediator.Send(new GetHealthQuery())));
app.MapPost("/api/identity/dev-login", async (DevLoginCommand command, IMediator mediator) =>
{
    var user = await mediator.Send(command);
    return user is null ? Results.Unauthorized() : Results.Ok(user);
});

app.Run();

public partial class Program;
