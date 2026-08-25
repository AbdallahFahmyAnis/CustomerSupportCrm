namespace Crm.Knowledge.Api.Domain;

/// <summary>SDD CRM-022 — lexical rank + snippet for an article.</summary>
public static class KnowledgeSearchRanker
{
    public static KnowledgeSearchHit? Rank(Article article, string query)
    {
        var term = query.Trim();
        if (term.Length == 0)
        {
            return null;
        }

        var titleHit = article.Title.Contains(term, StringComparison.OrdinalIgnoreCase);
        var bodyHit = article.Body.Contains(term, StringComparison.OrdinalIgnoreCase);
        if (!titleHit && !bodyHit)
        {
            return null;
        }

        var score = 0;
        if (titleHit)
        {
            score += 100;
        }

        if (bodyHit)
        {
            score += 10;
        }

        if (article.Status.Equals("Published", StringComparison.OrdinalIgnoreCase))
        {
            score += 1;
        }

        return new KnowledgeSearchHit(
            article.Id,
            article.Title,
            article.Kind,
            article.Status,
            score,
            BuildSnippet(article, term, titleHit, bodyHit),
            article.UpdatedAt);
    }

    private static string BuildSnippet(Article article, string term, bool titleHit, bool bodyHit)
    {
        if (bodyHit)
        {
            var idx = article.Body.IndexOf(term, StringComparison.OrdinalIgnoreCase);
            if (idx < 0)
            {
                return Trim(article.Body, 120);
            }

            var start = Math.Max(0, idx - 40);
            var length = Math.Min(article.Body.Length - start, 120);
            var slice = article.Body.Substring(start, length).Replace('\n', ' ').Trim();
            if (start > 0)
            {
                slice = "…" + slice;
            }

            if (start + length < article.Body.Length)
            {
                slice += "…";
            }

            return slice;
        }

        return titleHit ? article.Title : Trim(article.Body, 120);
    }

    private static string Trim(string value, int max) =>
        value.Length <= max ? value : value[..max].Trim() + "…";
}

/// <summary>SDD CRM-022 — ranked search hit (domain).</summary>
public sealed record KnowledgeSearchHit(
    Guid Id,
    string Title,
    string Kind,
    string Status,
    int Score,
    string Snippet,
    DateTimeOffset UpdatedAt);
