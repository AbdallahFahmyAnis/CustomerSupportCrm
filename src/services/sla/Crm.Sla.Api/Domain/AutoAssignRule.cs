namespace Crm.Sla.Api.Domain;

/// <summary>SDD CRM-018 — category/priority → agent rule.</summary>
public sealed class AutoAssignRule
{
    public Guid Id { get; private set; }
    public string? Category { get; private set; }
    public string? Priority { get; private set; }
    public string AgentId { get; private set; } = "";
    public string AgentName { get; private set; } = "";
    public bool Enabled { get; private set; }

    private AutoAssignRule()
    {
    }

    public static AutoAssignRule Create(
        string? category,
        string? priority,
        string agentId,
        string agentName,
        bool enabled = true,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(agentId) || string.IsNullOrWhiteSpace(agentName))
        {
            throw new ArgumentException("Agent id and name are required.");
        }

        if (priority is not null && !SlaCatalog.IsKnownPriority(priority))
        {
            throw new ArgumentException("Unknown priority.");
        }

        return new AutoAssignRule
        {
            Id = id ?? Guid.NewGuid(),
            Category = string.IsNullOrWhiteSpace(category) ? null : category.Trim(),
            Priority = priority is null ? null : SlaCatalog.NormalizePriority(priority),
            AgentId = agentId.Trim(),
            AgentName = agentName.Trim(),
            Enabled = enabled
        };
    }

    public int Specificity =>
        (Category is null ? 0 : 2) + (Priority is null ? 0 : 1);

    public bool Matches(string category, string priority)
    {
        if (!Enabled)
        {
            return false;
        }

        if (Category is not null &&
            !Category.Equals(category.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (Priority is not null &&
            !Priority.Equals(priority.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }
}
