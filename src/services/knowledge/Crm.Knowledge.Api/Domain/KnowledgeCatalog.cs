namespace Crm.Knowledge.Api.Domain;

/// <summary>SDD CRM-021 — article kind / status catalogs.</summary>
public static class KnowledgeCatalog
{
    public static readonly string[] Kinds = ["Faq", "Article", "Solution", "Guide"];
    public static readonly string[] Statuses = ["Draft", "Published"];
    public static readonly string[] Locales = ["en", "ar"];

    public static string NormalizeKind(string kind)
    {
        var match = Kinds.FirstOrDefault(k => k.Equals(kind.Trim(), StringComparison.OrdinalIgnoreCase));
        if (match is null)
        {
            throw new ArgumentException("Kind must be one of: " + string.Join(", ", Kinds));
        }

        return match;
    }

    public static string NormalizeStatus(string status)
    {
        var match = Statuses.FirstOrDefault(s => s.Equals(status.Trim(), StringComparison.OrdinalIgnoreCase));
        if (match is null)
        {
            throw new ArgumentException("Status must be one of: " + string.Join(", ", Statuses));
        }

        return match;
    }

    public static string NormalizeLocale(string? locale)
    {
        if (string.IsNullOrWhiteSpace(locale))
        {
            return "en";
        }

        var match = Locales.FirstOrDefault(l => l.Equals(locale.Trim(), StringComparison.OrdinalIgnoreCase));
        return match ?? "en";
    }
}
