namespace Crm.Tickets.Api.Domain;

/// <summary>SDD CRM-014 — follow-up task on a ticket.</summary>
public sealed class TicketTask
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string Title { get; private set; } = "";
    public DateTimeOffset? DueAt { get; private set; }
    public string? AssigneeUserId { get; private set; }
    public string? AssigneeName { get; private set; }
    public string Status { get; private set; } = "Open";
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private TicketTask() { }

    public static TicketTask Create(
        Guid ticketId,
        string title,
        DateTimeOffset? dueAt,
        string? assigneeUserId,
        string? assigneeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        var now = DateTimeOffset.UtcNow;
        return new TicketTask
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            Title = title.Trim(),
            DueAt = dueAt,
            AssigneeUserId = string.IsNullOrWhiteSpace(assigneeUserId) ? null : assigneeUserId.Trim(),
            AssigneeName = string.IsNullOrWhiteSpace(assigneeName) ? null : assigneeName.Trim(),
            Status = "Open",
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public static TicketTask Rehydrate(
        Guid id, Guid ticketId, string title, DateTimeOffset? dueAt,
        string? assigneeUserId, string? assigneeName, string status,
        DateTimeOffset createdAt, DateTimeOffset updatedAt) => new()
    {
        Id = id,
        TicketId = ticketId,
        Title = title,
        DueAt = dueAt,
        AssigneeUserId = assigneeUserId,
        AssigneeName = assigneeName,
        Status = status,
        CreatedAt = createdAt,
        UpdatedAt = updatedAt
    };

    public void Complete()
    {
        if (Status != "Open") throw new InvalidOperationException("Only open tasks can be completed.");
        Status = "Completed";
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status != "Open") throw new InvalidOperationException("Only open tasks can be cancelled.");
        Status = "Cancelled";
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
