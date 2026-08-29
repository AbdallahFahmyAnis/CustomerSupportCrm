using Microsoft.EntityFrameworkCore;

namespace Crm.Knowledge.Api.Infrastructure.Persistence;

/// <summary>SDD CRM-021 — EF Core knowledge schema.</summary>
public sealed class KnowledgeDbContext(DbContextOptions<KnowledgeDbContext> options) : DbContext(options)
{
    public DbSet<ArticleRow> Articles => Set<ArticleRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArticleRow>(e =>
        {
            e.ToTable("Articles");
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(300).IsRequired();
            e.Property(x => x.Body).IsRequired();
            e.Property(x => x.Kind).HasMaxLength(50).IsRequired();
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.Property(x => x.Locale).HasMaxLength(8).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(200).IsRequired();
        });
    }
}

public sealed class ArticleRow
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string Kind { get; set; } = "";
    public string Status { get; set; } = "";
    public string Locale { get; set; } = "en";
    public string CreatedBy { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
