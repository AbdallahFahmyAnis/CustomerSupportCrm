using Crm.BuildingBlocks.Diagnostics;
using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.AssignTicket;
using Crm.Tickets.Api.Features.ChangeStatus;
using Crm.Tickets.Api.Features.CreateTicket;
using Crm.Tickets.Api.Features.EscalateTicket;
using Crm.Tickets.Api.Features.GetHealth;
using Crm.Tickets.Api.Features.GetTicket;
using Crm.Tickets.Api.Features.GetTicketOptions;
using Crm.Tickets.Api.Features.SearchTickets;
using Crm.Tickets.Api.Features.UpdateClassification;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSingleton<TicketsDb>();

var app = builder.Build();
app.UseCorrelationId();

var db = app.Services.GetRequiredService<TicketsDb>();
db.EnsureSchema();
db.SeedIfEmpty();

static string Actor(HttpContext http) =>
    http.Request.Headers["X-Crm-User-Email"].FirstOrDefault()
    ?? http.Request.Headers["X-Crm-User-Id"].FirstOrDefault()
    ?? "Demo Agent";

app.MapGet("/health", async (IMediator mediator) => Results.Ok(await mediator.Send(new GetHealthQuery())));

app.MapGet("/api/tickets/options", async (IMediator mediator) =>
    Results.Ok(await mediator.Send(new GetTicketOptionsQuery())));

app.MapGet("/api/tickets", async (string? q, string? assignedTo, IMediator mediator) =>
    Results.Ok(await mediator.Send(new SearchTicketsQuery(q, assignedTo))));

app.MapGet("/api/tickets/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var ticket = await mediator.Send(new GetTicketQuery(id));
    return ticket is null ? Results.NotFound() : Results.Ok(ticket);
});

app.MapPost("/api/tickets", async (CreateTicketRequest body, HttpContext http, IMediator mediator) =>
{
    if (!Guid.TryParse(body.CustomerId, out var customerId))
    {
        return Results.BadRequest(new { error = "CustomerId must be a GUID." });
    }

    var result = await mediator.Send(new CreateTicketCommand(
        customerId,
        body.CustomerName,
        body.Subject,
        body.Description,
        body.Category,
        body.Priority,
        Actor(http)));
    return result.Error is not null
        ? Results.BadRequest(new { error = result.Error })
        : Results.Created($"/api/tickets/{result.Ticket!.Id}", result.Ticket);
});

app.MapPut("/api/tickets/{id:guid}/classification", async (Guid id, UpdateClassificationRequest body, HttpContext http, IMediator mediator) =>
{
    var result = await mediator.Send(new UpdateClassificationCommand(id, body.Category, body.Priority, Actor(http)));
    if (result.Error is null)
    {
        return Results.Ok(result.Ticket);
    }

    return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
        ? Results.NotFound(new { error = result.Error })
        : Results.BadRequest(new { error = result.Error });
});

app.MapPost("/api/tickets/{id:guid}/assign", async (Guid id, AssignTicketRequest body, HttpContext http, IMediator mediator) =>
{
    var result = await mediator.Send(new AssignTicketCommand(id, body.AgentId, body.AgentName, Actor(http)));
    if (result.Error is null)
    {
        return Results.Ok(result.Ticket);
    }

    return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
        ? Results.NotFound(new { error = result.Error })
        : Results.BadRequest(new { error = result.Error });
});

app.MapPost("/api/tickets/{id:guid}/status", async (Guid id, ChangeStatusRequest body, HttpContext http, IMediator mediator) =>
{
    var result = await mediator.Send(new ChangeStatusCommand(id, body.Status, Actor(http)));
    if (result.Error is null)
    {
        return Results.Ok(result.Ticket);
    }

    return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
        ? Results.NotFound(new { error = result.Error })
        : Results.BadRequest(new { error = result.Error });
});

app.MapPost("/api/tickets/{id:guid}/escalate", async (Guid id, EscalateTicketRequest body, HttpContext http, IMediator mediator) =>
{
    var result = await mediator.Send(new EscalateTicketCommand(id, body.AssignToAgentId, body.AssignToAgentName, Actor(http)));
    if (result.Error is null)
    {
        return Results.Ok(result.Ticket);
    }

    return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
        ? Results.NotFound(new { error = result.Error })
        : Results.BadRequest(new { error = result.Error });
});

app.Run();

public partial class Program;
