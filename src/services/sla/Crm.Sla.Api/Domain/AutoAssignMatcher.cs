namespace Crm.Sla.Api.Domain;

/// <summary>SDD CRM-018 — pick most specific matching assign rule.</summary>
public static class AutoAssignMatcher
{
    public static AutoAssignRule? Suggest(IEnumerable<AutoAssignRule> rules, string category, string priority)
    {
        return rules
            .Where(r => r.Matches(category, priority))
            .OrderByDescending(r => r.Specificity)
            .FirstOrDefault();
    }
}
