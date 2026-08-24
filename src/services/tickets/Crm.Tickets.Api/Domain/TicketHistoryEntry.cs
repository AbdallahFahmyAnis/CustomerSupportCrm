namespace Crm.Tickets.Api.Domain;

/// <summary>SDD CRM-007 — field-level history entry.</summary>
public sealed class TicketHistoryEntry
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string Field { get; private set; } = string.Empty;
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public string ChangedBy { get; private set; } = string.Empty;
    public DateTimeOffset ChangedAt { get; private set; }

    private TicketHistoryEntry()
    {
    }

    public static TicketHistoryEntry Create(
        Guid ticketId,
        string field,
        string? oldValue,
        string? newValue,
        string changedBy,
        DateTimeOffset changedAt)
        => new()
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            Field = field,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedBy = string.IsNullOrWhiteSpace(changedBy) ? "Agent" : changedBy.Trim(),
            ChangedAt = changedAt
        };

    public static TicketHistoryEntry Rehydrate(
        Guid id,
        Guid ticketId,
        string field,
        string? oldValue,
        string? newValue,
        string changedBy,
        DateTimeOffset changedAt)
        => new()
        {
            Id = id,
            TicketId = ticketId,
            Field = field,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedBy = changedBy,
            ChangedAt = changedAt
        };
}
