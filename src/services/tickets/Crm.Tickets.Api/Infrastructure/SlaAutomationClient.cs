using System.Net.Http.Json;
using Crm.Contracts.Sla;

namespace Crm.Tickets.Api.Infrastructure;

/// <summary>SDD CRM-018 / CRM-019 — fail-open SLA automation client.</summary>
public sealed class SlaAutomationClient(HttpClient http, ILogger<SlaAutomationClient> log)
{
    public async Task<SuggestAssigneeDto?> SuggestAssigneeAsync(
        string category,
        string priority,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await http.PostAsJsonAsync(
                "api/sla/suggest-assignee",
                new SuggestAssigneeRequest(category, priority),
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SuggestAssigneeDto>(cancellationToken);
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "SLA suggest-assignee failed; leaving ticket unassigned.");
            return null;
        }
    }

    public async Task<ShouldEscalateDto?> ShouldEscalateAsync(
        string priority,
        DateTimeOffset createdAt,
        bool isEscalated,
        string status,
        string? assignedAgentId,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await http.PostAsJsonAsync(
                "api/sla/should-escalate",
                new ShouldEscalateRequest(
                    priority,
                    createdAt,
                    isEscalated,
                    status,
                    assignedAgentId),
                cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ShouldEscalateDto>(cancellationToken);
        }
        catch (Exception ex)
        {
            log.LogWarning(ex, "SLA should-escalate failed; skipping escalation.");
            return null;
        }
    }
}
