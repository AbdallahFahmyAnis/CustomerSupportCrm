using System.Net.Http.Json;

namespace Crm.Tickets.Api.Infrastructure;

/// <summary>SDD CRM-016 — fail-open notifications producer for @mentions.</summary>
public sealed class NotificationsClient(HttpClient http, ILogger<NotificationsClient> log)
{
    public async Task NotifyMentionAsync(
        string userId,
        string ticketNumber,
        string ticketId,
        string authorName,
        string noteBody,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await http.PostAsJsonAsync(
                "api/notifications",
                new
                {
                    userId,
                    title = $"You were mentioned on {ticketNumber}",
                    body = Truncate($"{authorName}: {noteBody}", 280),
                    kind = "mention",
                    href = $"/agent/tickets/{ticketId}"
                },
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                log.LogWarning(
                    "Notifications create failed for {UserId}: {Status}",
                    userId,
                    response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "Notifications create failed for {UserId}; note was still saved.", userId);
        }
    }

    private static string Truncate(string value, int max)
        => value.Length <= max ? value : value[..(max - 1)] + "…";
}
