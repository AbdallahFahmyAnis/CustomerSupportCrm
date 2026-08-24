using Crm.BuildingBlocks.Diagnostics;
using Crm.Tickets.Api.Features.GetHealth;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();
app.UseCorrelationId();

app.MapGet("/health", async (IMediator mediator) => Results.Ok(await mediator.Send(new GetHealthQuery())));

app.Run();
