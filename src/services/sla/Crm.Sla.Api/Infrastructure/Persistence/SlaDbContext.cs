using Microsoft.EntityFrameworkCore;

namespace Crm.Sla.Api.Infrastructure.Persistence;

/// <summary>SDD CRM-017 — EF Core SLA schema.</summary>
public sealed class SlaDbContext(DbContextOptions<SlaDbContext> options) : DbContext(options)
{
    public DbSet<SlaPolicyRow> Policies => Set<SlaPolicyRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SlaPolicyRow>(e =>
        {
            e.ToTable("SlaPolicies");
            e.HasKey(x => x.Priority);
            e.Property(x => x.Priority).HasMaxLength(50);
        });
    }
}

public sealed class SlaPolicyRow
{
    public string Priority { get; set; } = "";
    public int FirstResponseMinutes { get; set; }
    public int ResolutionMinutes { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
