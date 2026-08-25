using Microsoft.EntityFrameworkCore;

namespace Crm.Tickets.Api.Infrastructure.Persistence;

/// <summary>SDD CRM-037 / specs/010 — EF Core tickets schema.</summary>
public sealed class TicketsDbContext(DbContextOptions<TicketsDbContext> options) : DbContext(options)
{
    public DbSet<TicketRow> Tickets => Set<TicketRow>();
    public DbSet<TicketHistoryRow> TicketHistory => Set<TicketHistoryRow>();
    public DbSet<TicketSequenceRow> TicketSequence => Set<TicketSequenceRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TicketRow>(e =>
        {
            e.ToTable("Tickets");
            e.HasKey(x => x.Id);
            e.Property(x => x.TicketNumber).HasMaxLength(32).IsRequired();
            e.HasIndex(x => x.TicketNumber).IsUnique();
            e.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Subject).HasMaxLength(500).IsRequired();
            e.Property(x => x.Category).HasMaxLength(100).IsRequired();
            e.Property(x => x.Priority).HasMaxLength(50).IsRequired();
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.Property(x => x.AssignedAgentId).HasMaxLength(64);
            e.Property(x => x.AssignedAgentName).HasMaxLength(200);
        });

        modelBuilder.Entity<TicketHistoryRow>(e =>
        {
            e.ToTable("TicketHistory");
            e.HasKey(x => x.Id);
            e.Property(x => x.Field).HasMaxLength(100).IsRequired();
            e.Property(x => x.ChangedBy).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.TicketId);
        });

        modelBuilder.Entity<TicketSequenceRow>(e =>
        {
            e.ToTable("TicketSequence");
            e.HasKey(x => x.Name);
            e.Property(x => x.Name).HasMaxLength(32);
        });
    }
}

public sealed class TicketRow
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = "";
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = "";
    public string Subject { get; set; } = "";
    public string? Description { get; set; }
    public string Category { get; set; } = "";
    public string Priority { get; set; } = "";
    public string Status { get; set; } = "";
    public string? AssignedAgentId { get; set; }
    public string? AssignedAgentName { get; set; }
    public bool IsEscalated { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class TicketHistoryRow
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public string Field { get; set; } = "";
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string ChangedBy { get; set; } = "";
    public DateTimeOffset ChangedAt { get; set; }
}

public sealed class TicketSequenceRow
{
    public string Name { get; set; } = "ticket";
    public long Value { get; set; }
}
