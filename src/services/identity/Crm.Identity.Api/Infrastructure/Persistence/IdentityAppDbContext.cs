using Crm.Identity.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Crm.Identity.Api.Infrastructure.Persistence;

/// <summary>SDD CRM-037 — Identity + OpenIddict EF Core DbContext.</summary>
public sealed class IdentityAppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options)
        : base(options)
    {
    }

    public DbSet<StoredRefreshToken> RefreshTokens => Set<StoredRefreshToken>();
    public DbSet<RevokedAccessToken> RevokedAccessTokens => Set<RevokedAccessToken>();
    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();
    public DbSet<SystemSettings> SystemSettings => Set<SystemSettings>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.UseOpenIddict();

        builder.Entity<ApplicationUser>(e =>
        {
            e.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();
        });

        builder.Entity<StoredRefreshToken>(e =>
        {
            e.ToTable("RefreshTokens");
            e.HasKey(x => x.Id);
            e.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
            e.HasIndex(x => x.TokenHash).IsUnique();
            e.Property(x => x.UserId).IsRequired();
        });

        builder.Entity<RevokedAccessToken>(e =>
        {
            e.ToTable("RevokedAccessTokens");
            e.HasKey(x => x.Jti);
            e.Property(x => x.Jti).HasMaxLength(100);
            e.Property(x => x.UserId).IsRequired();
        });

        builder.Entity<AuditLogEntry>(e =>
        {
            e.ToTable("AuditLogs");
            e.HasKey(x => x.Id);
            e.Property(x => x.Action).HasMaxLength(100).IsRequired();
            e.Property(x => x.ActorEmail).HasMaxLength(256);
            e.Property(x => x.TargetEmail).HasMaxLength(256);
            e.Property(x => x.Detail).HasMaxLength(1000);
            e.Property(x => x.OccurredAt).IsRequired();
            e.Property(x => x.Success).IsRequired();
        });

        builder.Entity<SystemSettings>(e =>
        {
            e.ToTable("SystemSettings");
            e.HasKey(x => x.Id);
            e.Property(x => x.OrganizationName).HasMaxLength(200).IsRequired();
            e.Property(x => x.SupportEmail).HasMaxLength(256).IsRequired();
            e.Property(x => x.DefaultCulture).HasMaxLength(10).IsRequired();
            e.Property(x => x.MaxFailedLoginAttempts).IsRequired();
            e.Property(x => x.LockoutMinutes).IsRequired();
            e.Property(x => x.UpdatedAt).IsRequired();
        });
    }
}
