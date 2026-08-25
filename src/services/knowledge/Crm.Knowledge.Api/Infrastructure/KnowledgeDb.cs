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
            if (db.Articles.Any())
            {
                return;
            }

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
        catch
        {
            // never brick startup
        }
    }

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

    public Article? Get(Guid id)
    {
        using var db = factory.CreateDbContext();
        var row = db.Articles.AsNoTracking().FirstOrDefault(a => a.Id == id);
        return row is null ? null : FromRow(row);
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
