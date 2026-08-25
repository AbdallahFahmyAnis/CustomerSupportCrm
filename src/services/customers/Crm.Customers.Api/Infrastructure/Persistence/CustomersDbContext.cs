using Microsoft.EntityFrameworkCore;

namespace Crm.Customers.Api.Infrastructure.Persistence;

/// <summary>SDD CRM-037 / specs/010 — EF Core customers schema.</summary>
public sealed class CustomersDbContext(DbContextOptions<CustomersDbContext> options) : DbContext(options)
{
    public DbSet<CustomerRow> Customers => Set<CustomerRow>();
    public DbSet<ContactRow> Contacts => Set<ContactRow>();
    public DbSet<NoteRow> Notes => Set<NoteRow>();
    public DbSet<AttachmentRow> Attachments => Set<AttachmentRow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CustomerRow>(e =>
        {
            e.ToTable("Customers");
            e.HasKey(x => x.Id);
            e.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            e.Property(x => x.UniqueIdentifier).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.UniqueIdentifier).IsUnique();
            e.Property(x => x.Organization).HasMaxLength(200);
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<ContactRow>(e =>
        {
            e.ToTable("Contacts");
            e.HasKey(x => x.Id);
            e.Property(x => x.Type).HasMaxLength(32).IsRequired();
            e.Property(x => x.Value).HasMaxLength(500).IsRequired();
            e.HasIndex(x => x.CustomerId);
        });

        modelBuilder.Entity<NoteRow>(e =>
        {
            e.ToTable("Notes");
            e.HasKey(x => x.Id);
            e.Property(x => x.Body).IsRequired();
            e.Property(x => x.AuthorName).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.CustomerId);
        });

        modelBuilder.Entity<AttachmentRow>(e =>
        {
            e.ToTable("Attachments");
            e.HasKey(x => x.Id);
            e.Property(x => x.FileName).HasMaxLength(260).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(200).IsRequired();
            e.Property(x => x.StoragePath).HasMaxLength(1000).IsRequired();
            e.HasIndex(x => x.CustomerId);
        });
    }
}

public sealed class CustomerRow
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = "";
    public string UniqueIdentifier { get; set; } = "";
    public string? Organization { get; set; }
    public string Status { get; set; } = "Active";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}

public sealed class ContactRow
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Type { get; set; } = "";
    public string Value { get; set; } = "";
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class NoteRow
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Body { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class AttachmentRow
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string FileName { get; set; } = "";
    public string ContentType { get; set; } = "";
    public long SizeBytes { get; set; }
    public string StoragePath { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
}
