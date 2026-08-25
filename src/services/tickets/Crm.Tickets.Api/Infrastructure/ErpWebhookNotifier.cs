using System.Net.Http.Json;
using System.Text.Json;
using Crm.Tickets.Api.Domain;

namespace Crm.Tickets.Api.Infrastructure;

/// <summary>SDD CRM-039 — best-effort ERP outbound webhook on ticket create.</summary>
public sealed class ErpWebhookNotifier(IHttpClientFactory httpFactory, IConfiguration config, ILogger<ErpWebhookNotifier> log)
{
    public static object BuildPayload(Ticket ticket) => new
    {
        ticketId = ticket.Id.ToString(),
        ticketNumber = ticket.TicketNumber,
        subject = ticket.Subject,
        status = ticket.Status,
        customerId = ticket.CustomerId.ToString()
    };

    public async Task NotifyTicketCreatedAsync(Ticket ticket, CancellationToken ct = default)
    {
        try
        {
            var client = httpFactory.CreateClient("erp");
            var identity = config["Services:Identity"] ?? "http://localhost:5101";
            using var settingsRes = await client.GetAsync($"{identity.TrimEnd('/')}/api/identity/integrations/erp", ct);
            if (!settingsRes.IsSuccessStatusCode)
            {
                return;
            }

            var body = await settingsRes.Content.ReadFromJsonAsync<ErpWebhookSettings>(cancellationToken: ct);
            var url = body?.WebhookUrl?.Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            var payload = BuildPayload(ticket);
            using var post = await client.PostAsJsonAsync(url, payload, ct);
            if (!post.IsSuccessStatusCode)
            {
                log.LogWarning("ERP webhook returned {Status}", post.StatusCode);
            }
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "ERP webhook failed (non-fatal)");
        }
    }

    private sealed record ErpWebhookSettings(string? WebhookUrl);
}
