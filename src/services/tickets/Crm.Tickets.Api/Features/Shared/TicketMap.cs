using System.Text.Json;
using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Domain;

namespace Crm.Tickets.Api.Features.Shared;

internal static class TicketMap
{
    public static TicketSummaryDto Summary(Ticket t) => new(
        t.Id.ToString(),
        t.TicketNumber,
        t.CustomerId.ToString(),
        t.CustomerName,
        t.Subject,
        t.Category,
        t.Priority,
        t.Status,
        t.AssignedAgentId,
        t.AssignedAgentName,
        t.IsEscalated,
        t.DepartmentId?.ToString());

    public static TicketDetailDto Detail(
        Ticket t,
        IReadOnlyList<TicketNote>? notes = null,
        TicketFeedback? feedback = null) => new(
        t.Id.ToString(),
        t.TicketNumber,
        t.CustomerId.ToString(),
        t.CustomerName,
        t.Subject,
        t.Description,
        t.Category,
        t.Priority,
        t.Status,
        t.AssignedAgentId,
        t.AssignedAgentName,
        t.IsEscalated,
        t.CreatedAt,
        t.UpdatedAt,
        t.History
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new TicketHistoryDto(
                h.Id.ToString(),
                h.Field,
                h.OldValue,
                h.NewValue,
                h.ChangedBy,
                h.ChangedAt))
            .ToList(),
        (notes ?? [])
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new TicketNoteDto(
                n.Id.ToString(),
                n.Body,
                n.AuthorName,
                n.AuthorUserId,
                n.MentionedUserIds,
                n.CreatedAt))
            .ToList(),
        feedback is null
            ? null
            : new TicketFeedbackDto(
                feedback.Id.ToString(),
                feedback.TicketId.ToString(),
                feedback.Rating,
                feedback.Comment,
                feedback.CreatedAt),
        t.DepartmentId?.ToString(),
        t.AiSummary,
        ParseHighlights(t.AiSummaryHighlightsJson),
        t.AiSummaryAt);

    static IReadOnlyList<string>? ParseHighlights(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(json);
        }
        catch
        {
            return null;
        }
    }
}

internal static class TicketHttp
{
    public static string Actor(HttpContext http) =>
        http.Request.Headers["X-Crm-User-Email"].FirstOrDefault()
        ?? http.Request.Headers["X-Crm-User-Id"].FirstOrDefault()
        ?? "Demo Agent";

    public static string? ActorUserId(HttpContext http) =>
        http.Request.Headers["X-Crm-User-Id"].FirstOrDefault();
}
