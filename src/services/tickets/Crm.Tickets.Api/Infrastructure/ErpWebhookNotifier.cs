using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json;
using Crm.Tickets.Api.Domain;

namespace Crm.Tickets.Api.Infrastructure;

/// <summary>SDD CRM-039 deferred / 048 — retries, auth header, durable outbox log.</summary>
public sealed class ErpWebhookNotifier(
    IHttpClientFactory httpFactory,
    IConfiguration config,
    ILogger<ErpWebhookNotifier> log)
{
    private readonly ConcurrentQueue<ErpDeliveryRecord> _deliveries = new();
    private const int MaxLog = 50;
    private readonly string? _outboxPath = ResolveOutboxPath(config);
    private bool _loaded;

    /// <summary>1 initial + 2 retries.</summary>
    public int MaxAttempts { get; set; } = 3;

    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(75);

    public static object BuildPayload(Ticket ticket) => new
    {
        ticketId = ticket.Id.ToString(),
        ticketNumber = ticket.TicketNumber,
        subject = ticket.Subject,
        status = ticket.Status,
        customerId = ticket.CustomerId.ToString()
    };

    public IReadOnlyList<ErpDeliveryRecord> RecentDeliveries(int take = 20)
    {
        EnsureLoaded();
        return _deliveries.Reverse().Take(Math.Clamp(take, 1, MaxLog)).ToList();
    }

    public async Task NotifyTicketCreatedAsync(Ticket ticket, CancellationToken ct = default)
    {
        try
        {
            var client = httpFactory.CreateClient("erp");
            var identity = config["Services:Identity"] ?? "http://localhost:5101";
            using var settingsRes = await client.GetAsync(
                $"{identity.TrimEnd('/')}/api/identity/integrations/erp", ct);
            if (!settingsRes.IsSuccessStatusCode)
            {
                return;
            }

            var body = await settingsRes.Content.ReadFromJsonAsync<ErpWebhookSettings>(cancellationToken: ct);
            await DeliverToUrlAsync(body?.WebhookUrl, ticket, body?.AuthHeader, ct);
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "ERP webhook failed (non-fatal)");
            Append(ticket.Id, "error");
        }
    }

    /// <summary>Empty URL is a no-op (CRM-039). Non-2xx / failure: up to 2 retries.</summary>
    public async Task DeliverToUrlAsync(
        string? url,
        Ticket ticket,
        string? authHeader = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        EnsureLoaded();
        var client = httpFactory.CreateClient("erp");
        var payload = BuildPayload(ticket);
        Exception? lastEx = null;
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                using var req = new HttpRequestMessage(HttpMethod.Post, url.Trim())
                {
                    Content = JsonContent.Create(payload)
                };
                if (!string.IsNullOrWhiteSpace(authHeader))
                {
                    req.Headers.TryAddWithoutValidation("Authorization", authHeader.Trim());
                }

                using var post = await client.SendAsync(req, ct);
                if (post.IsSuccessStatusCode)
                {
                    Append(ticket.Id, $"ok:{((int)post.StatusCode)}");
                    return;
                }

                lastEx = null;
                log.LogWarning(
                    "ERP webhook returned {Status} (attempt {Attempt}/{Max})",
                    post.StatusCode, attempt, MaxAttempts);
                if (attempt < MaxAttempts)
                {
                    await Task.Delay(RetryDelay, ct);
                    continue;
                }

                Append(ticket.Id, $"fail:{((int)post.StatusCode)}");
                return;
            }
            catch (Exception ex) when (attempt < MaxAttempts)
            {
                lastEx = ex;
                log.LogWarning(ex, "ERP webhook attempt {Attempt} failed", attempt);
                await Task.Delay(RetryDelay, ct);
            }
            catch (Exception ex)
            {
                lastEx = ex;
                log.LogWarning(ex, "ERP webhook failed after retries");
                Append(ticket.Id, "error");
                return;
            }
        }

        if (lastEx is not null)
        {
            Append(ticket.Id, "error");
        }
    }

    private void Append(Guid ticketId, string status)
    {
        EnsureLoaded();
        _deliveries.Enqueue(new ErpDeliveryRecord(ticketId.ToString(), status, DateTimeOffset.UtcNow));
        while (_deliveries.Count > MaxLog && _deliveries.TryDequeue(out _))
        {
        }

        Persist();
    }

    private void EnsureLoaded()
    {
        if (_loaded || string.IsNullOrWhiteSpace(_outboxPath))
        {
            _loaded = true;
            return;
        }

        try
        {
            if (File.Exists(_outboxPath))
            {
                var rows = JsonSerializer.Deserialize<List<ErpDeliveryRecord>>(File.ReadAllText(_outboxPath));
                if (rows is not null)
                {
                    foreach (var row in rows.TakeLast(MaxLog))
                    {
                        _deliveries.Enqueue(row);
                    }
                }
            }
        }
        catch
        {
            // ignore corrupt outbox
        }

        _loaded = true;
    }

    private void Persist()
    {
        if (string.IsNullOrWhiteSpace(_outboxPath))
        {
            return;
        }

        try
        {
            var dir = Path.GetDirectoryName(_outboxPath);
            if (!string.IsNullOrWhiteSpace(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllText(_outboxPath, JsonSerializer.Serialize(_deliveries.ToList()));
        }
        catch
        {
            // non-fatal
        }
    }

    private static string? ResolveOutboxPath(IConfiguration config)
    {
        var explicitPath = config["Tickets:ErpOutboxPath"];
        if (!string.IsNullOrWhiteSpace(explicitPath))
        {
            return explicitPath;
        }

        var data = config["Tickets:DataPath"];
        return string.IsNullOrWhiteSpace(data)
            ? null
            : Path.Combine(data, "erp-outbox.json");
    }

    private sealed record ErpWebhookSettings(string? WebhookUrl, string? AuthHeader = null);
}

public sealed record ErpDeliveryRecord(string TicketId, string Status, DateTimeOffset At);
