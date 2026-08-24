using Crm.BuildingBlocks.Diagnostics;
using Crm.Contracts.Customers;
using Crm.Customers.Api.Features.AddAttachment;
using Crm.Customers.Api.Features.AddContact;
using Crm.Customers.Api.Features.AddNote;
using Crm.Customers.Api.Features.CreateCustomer;
using Crm.Customers.Api.Features.DeactivateContact;
using Crm.Customers.Api.Features.GetAttachment;
using Crm.Customers.Api.Features.GetBootstrapStatus;
using Crm.Customers.Api.Features.GetCustomer;
using Crm.Customers.Api.Features.GetHealth;
using Crm.Customers.Api.Features.SearchCustomers;
using Crm.Customers.Api.Features.UpdateCustomer;
using Crm.Customers.Api.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSingleton<CustomersDb>();

var app = builder.Build();
app.UseCorrelationId();

var db = app.Services.GetRequiredService<CustomersDb>();
db.EnsureSchema();
db.SeedIfEmpty();

app.MapGet("/health", async (IMediator mediator) => Results.Ok(await mediator.Send(new GetHealthQuery())));
app.MapGet("/api/customers/bootstrap", async (IMediator mediator) =>
    Results.Ok(await mediator.Send(new GetBootstrapStatusQuery())));

app.MapGet("/api/customers", async (string? q, IMediator mediator) =>
    Results.Ok(await mediator.Send(new SearchCustomersQuery(q))));

app.MapGet("/api/customers/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var customer = await mediator.Send(new GetCustomerQuery(id));
    return customer is null ? Results.NotFound() : Results.Ok(customer);
});

app.MapPost("/api/customers", async (CreateCustomerRequest body, IMediator mediator) =>
{
    var result = await mediator.Send(new CreateCustomerCommand(
        body.DisplayName,
        body.UniqueIdentifier,
        body.Organization,
        body.Status));
    if (result.Duplicate is not null)
    {
        return Results.Conflict(result.Duplicate);
    }

    return Results.Created($"/api/customers/{result.Customer!.Id}", result.Customer);
});

app.MapPut("/api/customers/{id:guid}", async (Guid id, UpdateCustomerRequest body, IMediator mediator) =>
{
    var result = await mediator.Send(new UpdateCustomerCommand(
        id,
        body.DisplayName,
        body.UniqueIdentifier,
        body.Organization,
        body.Status));
    if (result.Error is not null)
    {
        return Results.NotFound(new { error = result.Error });
    }

    if (result.Duplicate is not null)
    {
        return Results.Conflict(result.Duplicate);
    }

    return Results.Ok(result.Customer);
});

app.MapPost("/api/customers/{id:guid}/contacts", async (Guid id, AddContactRequest body, IMediator mediator) =>
{
    var result = await mediator.Send(new AddContactCommand(id, body.Type, body.Value, body.IsPrimary));
    if (result.Error is not null)
    {
        return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
            ? Results.NotFound(new { error = result.Error })
            : Results.BadRequest(new { error = result.Error });
    }

    return Results.Created($"/api/customers/{id}/contacts/{result.Contact!.Id}", result.Contact);
});

app.MapPost("/api/customers/{id:guid}/contacts/{contactId:guid}/deactivate", async (Guid id, Guid contactId, IMediator mediator) =>
{
    var result = await mediator.Send(new DeactivateContactCommand(id, contactId));
    return result.Ok ? Results.NoContent() : Results.NotFound(new { error = result.Error });
});

app.MapPost("/api/customers/{id:guid}/notes", async (Guid id, AddNoteRequest body, HttpContext http, IMediator mediator) =>
{
    var author = http.Request.Headers["X-Crm-User-Email"].FirstOrDefault()
        ?? http.Request.Headers["X-Crm-User-Id"].FirstOrDefault()
        ?? "Demo Agent";
    var result = await mediator.Send(new AddNoteCommand(id, body.Body, author));
    if (result.Error is not null)
    {
        return result.Error.Contains("not found", StringComparison.OrdinalIgnoreCase)
            ? Results.NotFound(new { error = result.Error })
            : Results.BadRequest(new { error = result.Error });
    }

    return Results.Created($"/api/customers/{id}/notes/{result.Note!.Id}", result.Note);
});

app.MapPost("/api/customers/{id:guid}/attachments", async (Guid id, HttpRequest request, IMediator mediator) =>
{
    if (!request.HasFormContentType)
    {
        return Results.BadRequest(new { error = "multipart form required" });
    }

    var form = await request.ReadFormAsync();
    var file = form.Files.GetFile("file") ?? form.Files.FirstOrDefault();
    if (file is null || file.Length == 0)
    {
        return Results.BadRequest(new { error = "file is required" });
    }

    await using var stream = file.OpenReadStream();
    var result = await mediator.Send(new AddAttachmentCommand(id, file.FileName, file.ContentType, stream));
    if (result.Error is not null)
    {
        return Results.NotFound(new { error = result.Error });
    }

    return Results.Created($"/api/customers/{id}/attachments/{result.Attachment!.Id}", result.Attachment);
});

app.MapGet("/api/customers/{id:guid}/attachments/{attachmentId:guid}", async (Guid id, Guid attachmentId, IMediator mediator) =>
{
    var result = await mediator.Send(new GetAttachmentQuery(id, attachmentId));
    return result is null
        ? Results.NotFound()
        : Results.File(result.StoragePath, result.ContentType, result.FileName);
});

app.Run();

public partial class Program;
