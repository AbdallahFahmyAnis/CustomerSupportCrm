namespace Crm.Tickets.Api.Domain;

/// <summary>SDD CRM-016 — parse @Agent Name mentions against the ticket catalog.</summary>
public static class MentionParser
{
    public static IReadOnlyList<(string Id, string Name)> Parse(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return [];
        }

        var matches = new List<(string Id, string Name)>();
        foreach (var agent in TicketCatalog.Agents.OrderByDescending(a => a.Name.Length))
        {
            var token = "@" + agent.Name;
            if (body.Contains(token, StringComparison.OrdinalIgnoreCase) &&
                matches.All(m => !m.Id.Equals(agent.Id, StringComparison.OrdinalIgnoreCase)))
            {
                matches.Add(agent);
            }
        }

        return matches;
    }
}
