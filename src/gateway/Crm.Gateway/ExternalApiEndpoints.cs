using System.Net.Http.Headers;
using System.Text;
using Crm.BuildingBlocks.Security;

namespace Crm.Gateway;

/// <summary>SDD CRM-038 — forward authenticated external calls to downstream services.</summary>
public static class ExternalApiEndpoints
{
    public static void Map(WebApplication app)
    {
        var group = app.MapGroup("/api/external/v1");

        // SDD CRM-038 polish / 041 — public OpenAPI (no API key).
        group.MapGet("/openapi.yaml", () =>
            Results.Text(ExternalApiOpenApi.Yaml, "application/yaml"));

        group.MapPost("/tickets", async (HttpRequest request, IHttpClientFactory httpFactory, IConfiguration config) =>
        {
            if (!Authorize(request, config))
            {
                return Results.Unauthorized();
            }

            var tickets = config["Services:Tickets"] ?? "http://localhost:5103";
            return await Forward(httpFactory, HttpMethod.Post, $"{tickets.TrimEnd('/')}/api/tickets", request);
        });

        group.MapGet("/tickets/{id:guid}", async (Guid id, HttpRequest request, IHttpClientFactory httpFactory, IConfiguration config) =>
        {
            if (!Authorize(request, config))
            {
                return Results.Unauthorized();
            }

            var tickets = config["Services:Tickets"] ?? "http://localhost:5103";
            return await Forward(httpFactory, HttpMethod.Get, $"{tickets.TrimEnd('/')}/api/tickets/{id}", request);
        });

        group.MapGet("/customers", async (string? q, HttpRequest request, IHttpClientFactory httpFactory, IConfiguration config) =>
        {
            if (!Authorize(request, config))
            {
                return Results.Unauthorized();
            }

            var customers = config["Services:Customers"] ?? "http://localhost:5102";
            var qs = string.IsNullOrWhiteSpace(q) ? "" : $"?q={Uri.EscapeDataString(q.Trim())}";
            return await Forward(httpFactory, HttpMethod.Get, $"{customers.TrimEnd('/')}/api/customers{qs}", request);
        });
    }

    static bool Authorize(HttpRequest request, IConfiguration config)
    {
        request.Headers.TryGetValue("X-Api-Key", out var keyHeader);
        var auth = request.Headers.Authorization.ToString();
        return ExternalApiKey.IsAuthorized(config["ExternalApi:ApiKey"], keyHeader.ToString(), auth);
    }

    static async Task<IResult> Forward(
        IHttpClientFactory httpFactory,
        HttpMethod method,
        string url,
        HttpRequest incoming)
    {
        var client = httpFactory.CreateClient("downstream");
        using var outbound = new HttpRequestMessage(method, url);
        if (method != HttpMethod.Get && incoming.ContentLength is null or > 0)
        {
            using var reader = new StreamReader(incoming.Body, Encoding.UTF8);
            var body = await reader.ReadToEndAsync();
            if (!string.IsNullOrEmpty(body))
            {
                outbound.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
        }

        if (incoming.ContentType is not null && outbound.Content is not null)
        {
            outbound.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(incoming.ContentType);
        }

        using var response = await client.SendAsync(outbound);
        var responseBody = await response.Content.ReadAsStringAsync();
        var media = response.Content.Headers.ContentType?.ToString() ?? "application/json";
        return Results.Content(responseBody, media, statusCode: (int)response.StatusCode);
    }
}
