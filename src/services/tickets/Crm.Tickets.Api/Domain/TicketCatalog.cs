namespace Crm.Tickets.Api.Domain;

public static class TicketCatalog
{
    public static readonly string[] Categories = ["Billing", "Technical", "General"];
    public static readonly string[] Priorities = ["Low", "Medium", "High", "Urgent"];

    public static readonly (string Id, string Name)[] Agents =
    [
        ("11111111-1111-1111-1111-111111111111", "Demo Agent"),
        ("22222222-2222-2222-2222-222222222222", "Lead Agent")
    ];

    public static void EnsureCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category) ||
            !Categories.Contains(category.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Category is required and must be one of: " + string.Join(", ", Categories));
        }
    }

    public static void EnsurePriority(string priority)
    {
        if (string.IsNullOrWhiteSpace(priority) ||
            !Priorities.Contains(priority.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Priority is required and must be one of: " + string.Join(", ", Priorities));
        }
    }
}

public static class TicketStatuses
{
    public const string New = "New";
    public const string InProgress = "InProgress";
    public const string Waiting = "Waiting";
    public const string Resolved = "Resolved";
    public const string Closed = "Closed";

    public static readonly string[] All = [New, InProgress, Waiting, Resolved, Closed];

    private static readonly Dictionary<string, string[]> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        [New] = [InProgress, Waiting, Closed],
        [InProgress] = [Waiting, Resolved, Closed],
        [Waiting] = [InProgress, Resolved, Closed],
        [Resolved] = [Closed, InProgress],
        [Closed] = []
    };

    public static void EnsureTransition(string current, string next)
    {
        if (string.IsNullOrWhiteSpace(next) || !All.Contains(next, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unknown status '{next}'.");
        }

        if (current.Equals(next, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!Allowed.TryGetValue(current, out var nexts) ||
            !nexts.Contains(next, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Status transition from '{current}' to '{next}' is not allowed.");
        }
    }
}
