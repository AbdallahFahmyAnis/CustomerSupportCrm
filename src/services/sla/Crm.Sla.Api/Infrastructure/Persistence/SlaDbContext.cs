using Microsoft.EntityFrameworkCore;

namespace Crm.Sla.Api.Infrastructure.Persistence;

/// <summary>SDD CRM-017 / CRM-018 / CRM-019 — EF Core SLA schema.</summary>
public sealed class SlaDbContext(DbContextOptions<SlaDbContext> options) : DbContext(options)
{
    public DbSet<SlaPolicyRow> Policies => Set<SlaPolicyRow>();
    public DbSet<AutoAssignRuleRow> AssignRules => Set<AutoAssignRuleRow>();
    public DbSet<EscalationSettingsRow> EscalationSettings => Set<EscalationSettingsRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SlaPolicyRow>(e =>
        {
            e.ToTable("SlaPolicies");
            e.HasKey(x => x.Priority);
            e.Property(x => x.Priority).HasMaxLength(50);
        });

        modelBuilder.Entity<AutoAssignRuleRow>(e =>
        {
            e.ToTable("AutoAssignRules");
            e.HasKey(x => x.Id);
            e.Property(x => x.Category).HasMaxLength(100);
            e.Property(x => x.Priority).HasMaxLength(50);
            e.Property(x => x.AgentId).HasMaxLength(64).IsRequired();
            e.Property(x => x.AgentName).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<EscalationSettingsRow>(e =>
        {
            e.ToTable("EscalationSettings");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).HasMaxLength(32);
            e.Property(x => x.AssignToAgentId).HasMaxLength(64).IsRequired();
            e.Property(x => x.AssignToAgentName).HasMaxLength(200).IsRequired();
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

public sealed class AutoAssignRuleRow
{
    public Guid Id { get; set; }
    public string? Category { get; set; }
    public string? Priority { get; set; }
    public string AgentId { get; set; } = "";
    public string AgentName { get; set; } = "";
    public bool Enabled { get; set; } = true;
}

public sealed class EscalationSettingsRow
{
    public string Id { get; set; } = "default";
    public bool EscalateOnFirstResponseBreach { get; set; }
    public bool EscalateOnResolutionBreach { get; set; }
    public bool EscalateUrgentAlways { get; set; }
    public string AssignToAgentId { get; set; } = "";
    public string AssignToAgentName { get; set; } = "";
    public DateTimeOffset UpdatedAt { get; set; }
}
