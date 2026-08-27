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

    /// <summary>SDD CRM-015 — shared quick reply library.</summary>
    public static readonly (string Id, string Title, string Body)[] QuickReplies =
    [
        ("qr-billing-apology", "Billing apology",
            "I'm sorry for the confusion on your invoice. I'll review the line items and get back to you shortly."),
        ("qr-password-reset", "Password reset steps",
            "Please use the Forgot password link on the sign-in page, then check your inbox (and spam) for the reset email."),
        ("qr-escalate", "Escalation notice",
            "I've escalated your request to a senior agent. Someone will follow up within one business day."),
        ("qr-waiting-info", "Waiting on customer",
            "Thanks — to continue, could you reply with the account number and a screenshot of the error?"),
        ("qr-resolved", "Resolution confirmation",
            "I've applied the fix on our side. Please confirm everything looks good so we can close this ticket.")
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
