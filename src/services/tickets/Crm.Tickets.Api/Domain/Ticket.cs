namespace Crm.Tickets.Api.Domain;

/// <summary>SDD CRM-004…007 / specs/003-ticket-lifecycle — ticket aggregate.</summary>
public sealed class Ticket
{
    private readonly List<TicketHistoryEntry> _history = [];

    public Guid Id { get; private set; }
    public string TicketNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string Priority { get; private set; } = string.Empty;
    public string Status { get; private set; } = TicketStatuses.New;
    public string? AssignedAgentId { get; private set; }
    public string? AssignedAgentName { get; private set; }
    public bool IsEscalated { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyList<TicketHistoryEntry> History => _history;

    private Ticket()
    {
    }

    public static Ticket Create(
        string ticketNumber,
        Guid customerId,
        string customerName,
        string subject,
        string? description,
        string category,
        string priority,
        string actor)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticketNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        TicketCatalog.EnsureCategory(category);
        TicketCatalog.EnsurePriority(priority);

        var now = DateTimeOffset.UtcNow;
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            TicketNumber = ticketNumber.Trim(),
            CustomerId = customerId,
            CustomerName = customerName.Trim(),
            Subject = subject.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Category = category.Trim(),
            Priority = priority.Trim(),
            Status = TicketStatuses.New,
            CreatedAt = now,
            UpdatedAt = now
        };
        ticket.Record("Status", null, ticket.Status, actor, now);
        ticket.Record("Category", null, ticket.Category, actor, now);
        ticket.Record("Priority", null, ticket.Priority, actor, now);
        return ticket;
    }

    public static Ticket Rehydrate(
        Guid id,
        string ticketNumber,
        Guid customerId,
        string customerName,
        string subject,
        string? description,
        string category,
        string priority,
        string status,
        string? assignedAgentId,
        string? assignedAgentName,
        bool isEscalated,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt,
        IEnumerable<TicketHistoryEntry>? history = null,
        Guid? departmentId = null)
    {
        var ticket = new Ticket
        {
            Id = id,
            TicketNumber = ticketNumber,
            CustomerId = customerId,
            CustomerName = customerName,
            Subject = subject,
            Description = description,
            Category = category,
            Priority = priority,
            Status = status,
            AssignedAgentId = assignedAgentId,
            AssignedAgentName = assignedAgentName,
            IsEscalated = isEscalated,
            DepartmentId = departmentId,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
        if (history is not null)
        {
            ticket._history.AddRange(history);
        }

        return ticket;
    }

    /// <summary>SDD CRM-043</summary>
    public void SetDepartment(Guid? departmentId)
    {
        DepartmentId = departmentId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Classify(string category, string priority, string actor)
    {
        TicketCatalog.EnsureCategory(category);
        TicketCatalog.EnsurePriority(priority);
        var now = DateTimeOffset.UtcNow;
        if (!Category.Equals(category, StringComparison.OrdinalIgnoreCase))
        {
            Record("Category", Category, category.Trim(), actor, now);
            Category = category.Trim();
        }

        if (!Priority.Equals(priority, StringComparison.OrdinalIgnoreCase))
        {
            Record("Priority", Priority, priority.Trim(), actor, now);
            Priority = priority.Trim();
        }

        UpdatedAt = now;
    }

    public void Assign(string? agentId, string? agentName, string actor)
    {
        var now = DateTimeOffset.UtcNow;
        var newId = string.IsNullOrWhiteSpace(agentId) ? null : agentId.Trim();
        var newName = string.IsNullOrWhiteSpace(agentName) ? null : agentName.Trim();
        if (newId is not null && newName is null)
        {
            throw new ArgumentException("Agent name is required when assigning.");
        }

        Record("AssignedAgent", FormatAssignee(AssignedAgentId, AssignedAgentName), FormatAssignee(newId, newName), actor, now);
        AssignedAgentId = newId;
        AssignedAgentName = newName;
        UpdatedAt = now;
    }

    public void ChangeStatus(string nextStatus, string actor)
    {
        TicketStatuses.EnsureTransition(Status, nextStatus);
        var now = DateTimeOffset.UtcNow;
        Record("Status", Status, nextStatus, actor, now);
        Status = nextStatus;
        UpdatedAt = now;
    }

    public void Escalate(string? assignToAgentId, string? assignToAgentName, string actor)
    {
        var now = DateTimeOffset.UtcNow;
        if (!IsEscalated)
        {
            Record("Escalated", "false", "true", actor, now);
            IsEscalated = true;
        }

        if (!string.IsNullOrWhiteSpace(assignToAgentId))
        {
            Assign(assignToAgentId, assignToAgentName, actor);
        }
        else
        {
            UpdatedAt = now;
        }
    }

    private void Record(string field, string? oldValue, string? newValue, string actor, DateTimeOffset at)
        => _history.Add(TicketHistoryEntry.Create(Id, field, oldValue, newValue, actor, at));

    private static string? FormatAssignee(string? id, string? name)
        => id is null ? null : $"{name} ({id})";
}
