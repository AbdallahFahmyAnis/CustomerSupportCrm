namespace Crm.Tickets.Api.Domain;

/// <summary>SDD CRM-016 — internal collaboration note on a ticket.</summary>
public sealed class TicketNote
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string Body { get; private set; } = string.Empty;
    public string AuthorName { get; private set; } = string.Empty;
    public string? AuthorUserId { get; private set; }
    public IReadOnlyList<string> MentionedUserIds { get; private set; } = [];
    public DateTimeOffset CreatedAt { get; private set; }

    private TicketNote()
    {
    }

    public static TicketNote Create(
        Guid ticketId,
        string body,
        string authorName,
        string? authorUserId,
        IReadOnlyList<string> mentionedUserIds)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(body);
        return new TicketNote
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            Body = body.Trim(),
            AuthorName = string.IsNullOrWhiteSpace(authorName) ? "Agent" : authorName.Trim(),
            AuthorUserId = string.IsNullOrWhiteSpace(authorUserId) ? null : authorUserId.Trim(),
            MentionedUserIds = mentionedUserIds.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static TicketNote Rehydrate(
        Guid id,
        Guid ticketId,
        string body,
        string authorName,
        string? authorUserId,
        IEnumerable<string> mentionedUserIds,
        DateTimeOffset createdAt)
        => new()
        {
            Id = id,
            TicketId = ticketId,
            Body = body,
            AuthorName = authorName,
            AuthorUserId = authorUserId,
            MentionedUserIds = mentionedUserIds.ToList(),
            CreatedAt = createdAt
        };
}
