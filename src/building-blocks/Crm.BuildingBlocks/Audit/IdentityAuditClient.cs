using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Crm.BuildingBlocks.Audit;

/// <summary>SDD CRM-036 / specs/051 — fail-open audit writer to Identity.</summary>
public sealed class IdentityAuditClient(HttpClient http, ILogger<IdentityAuditClient> log)
{
    public async Task WriteAsync(
        string service,
        string action,
        bool success,
        string? actorEmail,
        string? targetEmail,
        string? detail,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "api/identity/audit");
            request.Headers.TryAddWithoutValidation("X-Crm-Audit-Service", service);
            request.Content = JsonContent.Create(new
            {
                action,
                success,
                actorEmail,
                targetEmail,
                detail,
                service
            });
            var response = await http.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                log.LogWarning("Audit ingest failed: {Status} {Service}/{Action}", response.StatusCode, service, action);
            }
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Audit ingest failed for {Service}/{Action}; business op continues.", service, action);
        }
    }
}

public static class AuditServices
{
    public const string Identity = "Identity";
    public const string Customers = "Customers";
    public const string Tickets = "Tickets";
    public const string Knowledge = "Knowledge";
    public const string Sla = "Sla";
    public const string Channels = "Channels";
}
