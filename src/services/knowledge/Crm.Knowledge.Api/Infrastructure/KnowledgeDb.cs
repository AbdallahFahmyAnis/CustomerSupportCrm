using Crm.Knowledge.Api.Domain;
using Crm.Knowledge.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Crm.Knowledge.Api.Infrastructure;

/// <summary>SDD CRM-021 — EF Core facade for articles.</summary>
public sealed class KnowledgeDb(IDbContextFactory<KnowledgeDbContext> factory)
{
    public void EnsureSchema()
    {
        using var db = factory.CreateDbContext();
        db.Database.EnsureCreated();
    }

    public void SeedIfEmpty()
    {
        try
        {
            using var db = factory.CreateDbContext();
            if (!db.Articles.Any())
            {
                db.Articles.Add(ToRow(Article.Create(
                    "How do I reset my portal password?",
                    "Open the portal sign-in page, choose Forgot password, and follow the email link within 30 minutes.",
                    "Faq",
                    "Published",
                    "System")));
                db.Articles.Add(ToRow(Article.Create(
                    "Invoice mismatch troubleshooting",
                    "1. Confirm the PO number on the ticket.\n2. Compare line items with the latest invoice PDF.\n3. If tax differs, escalate Billing with screenshots.",
                    "Solution",
                    "Published",
                    "System")));
                db.SaveChanges();
            }

            EnsurePortalFaqs(db);
        }
        catch
        {
            // never brick startup
        }
    }

    /// <summary>Idempotent portal FAQ catalog (adds missing titles only).</summary>
    private static void EnsurePortalFaqs(KnowledgeDbContext db)
    {
        var existing = db.Articles
            .Where(a => a.Kind == "Faq")
            .Select(a => a.Title)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var (title, body) in PortalFaqSeeds)
        {
            if (existing.Contains(title))
            {
                continue;
            }

            db.Articles.Add(ToRow(Article.Create(title, body, "Faq", "Published", "System")));
            existing.Add(title);
        }

        db.SaveChanges();
    }

    private static readonly (string Title, string Body)[] PortalFaqSeeds =
    [
        ("How do I reset my portal password?",
            "Open the portal sign-in page, choose Forgot password, and follow the email link within 30 minutes. Check spam if the mail is delayed."),
        ("How do I track an existing support request?",
            "Open Track my requests in the portal and search with the email you used when submitting. Signed-in customers see their requests automatically."),
        ("How do I start a live chat with support?",
            "Choose Live chat from the portal menu. Signed-in customers skip name and email — just send a message. Your chat becomes a support ticket."),
        ("Where can I rate support after a ticket closes?",
            "Open Rate support and enter your ticket number, or use the Rate support link from Track my requests or after Live chat / the assistant."),
        ("What should I ask the support assistant?",
            "Ask about passwords, billing, invoices, tracking tickets, or live chat. If you need a person, say “human agent” or open Submit a request."),
        ("How do billing and invoice questions get handled?",
            "Include your invoice or PO number when you submit a request or chat. Agents compare line items and tax with Finance when needed."),
        ("How do I submit a new support request?",
            "Use Submit a request on the portal home. Provide a clear subject and description so agents can triage faster."),
        ("Can I continue an earlier live chat?",
            "Yes — reopen Live chat while signed in. The portal restores your open chat ticket and shows agent replies as they arrive."),
    ];

    public IReadOnlyList<Article> Search(string? q)
    {
        using var db = factory.CreateDbContext();
        var rows = db.Articles.AsNoTracking().ToList();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            rows = rows
                .Where(a =>
                    a.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    a.Body.Contains(term, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return rows.OrderByDescending(a => a.UpdatedAt).Select(FromRow).ToList();
    }

    /// <summary>SDD CRM-022 — ranked search with optional filters.</summary>
    public IReadOnlyList<KnowledgeSearchHit> RankedSearch(
        string query,
        string? kind,
        string? status,
        bool publishedOnly)
    {
        using var db = factory.CreateDbContext();
        IEnumerable<ArticleRow> rows = db.Articles.AsNoTracking().ToList();
        if (!string.IsNullOrWhiteSpace(kind))
        {
            rows = rows.Where(a => a.Kind.Equals(kind.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (publishedOnly)
        {
            rows = rows.Where(a => a.Status.Equals("Published", StringComparison.OrdinalIgnoreCase));
        }
        else if (!string.IsNullOrWhiteSpace(status))
        {
            rows = rows.Where(a => a.Status.Equals(status.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return rows
            .Select(FromRow)
            .Select(a => KnowledgeSearchRanker.Rank(a, query))
            .Where(h => h is not null)
            .Select(h => h!)
            .OrderByDescending(h => h.Score)
            .ThenByDescending(h => h.UpdatedAt)
            .ToList();
    }

    public Article? Get(Guid id)
    {
        using var db = factory.CreateDbContext();
        var row = db.Articles.AsNoTracking().FirstOrDefault(a => a.Id == id);
        return row is null ? null : FromRow(row);
    }

    /// <summary>SDD CRM-029 — published FAQ summaries for the customer portal.</summary>
    public IReadOnlyList<Article> ListPortalFaqs(string? q)
    {
        using var db = factory.CreateDbContext();
        IEnumerable<ArticleRow> rows = db.Articles.AsNoTracking().ToList()
            .Where(a =>
                a.Kind.Equals("Faq", StringComparison.OrdinalIgnoreCase) &&
                a.Status.Equals("Published", StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(q))
        {
            var terms = q.Trim()
                .Split([' ', '\t', '\r', '\n', ',', '.', '?', '!'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(t => t.Length > 2)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            if (terms.Length > 0)
            {
                rows = rows.Where(a =>
                    terms.Any(term =>
                        a.Title.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                        a.Body.Contains(term, StringComparison.OrdinalIgnoreCase)));
            }
        }

        return rows.OrderByDescending(a => a.UpdatedAt).Select(FromRow).ToList();
    }

    /// <summary>SDD CRM-029 — published FAQ detail only (null if Draft or non-Faq).</summary>
    public Article? GetPortalFaq(Guid id)
    {
        var article = Get(id);
        if (article is null)
        {
            return null;
        }

        if (!article.Kind.Equals("Faq", StringComparison.OrdinalIgnoreCase) ||
            !article.Status.Equals("Published", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return article;
    }

    public void Insert(Article article)
    {
        using var db = factory.CreateDbContext();
        db.Articles.Add(ToRow(article));
        db.SaveChanges();
    }

    public void Update(Article article)
    {
        using var db = factory.CreateDbContext();
        var row = db.Articles.FirstOrDefault(a => a.Id == article.Id)
                  ?? throw new InvalidOperationException("Article not found.");
        row.Title = article.Title;
        row.Body = article.Body;
        row.Kind = article.Kind;
        row.Status = article.Status;
        row.UpdatedAt = article.UpdatedAt;
        db.SaveChanges();
    }

    private static ArticleRow ToRow(Article article) => new()
    {
        Id = article.Id,
        Title = article.Title,
        Body = article.Body,
        Kind = article.Kind,
        Status = article.Status,
        CreatedBy = article.CreatedBy,
        CreatedAt = article.CreatedAt,
        UpdatedAt = article.UpdatedAt
    };

    private static Article FromRow(ArticleRow row) =>
        Article.Rehydrate(
            row.Id,
            row.Title,
            row.Body,
            row.Kind,
            row.Status,
            row.CreatedBy,
            row.CreatedAt,
            row.UpdatedAt);
}
