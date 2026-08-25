using Crm.Sla.Api.Domain;
using Crm.Sla.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Crm.Sla.Api.Infrastructure;

/// <summary>SDD CRM-017 — EF Core facade for SLA policies.</summary>
public sealed class SlaDb(IDbContextFactory<SlaDbContext> factory)
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
            if (db.Policies.Any())
            {
                return;
            }

            foreach (var policy in DefaultPolicies())
            {
                db.Policies.Add(ToRow(policy));
            }

            db.SaveChanges();
        }
        catch
        {
            // never brick startup
        }
    }

    public IReadOnlyList<SlaPolicy> ListPolicies()
    {
        using var db = factory.CreateDbContext();
        var rows = db.Policies.AsNoTracking().ToList();
        return rows
            .OrderBy(p => Array.IndexOf(SlaCatalog.Priorities, p.Priority))
            .Select(FromRow)
            .ToList();
    }

    public SlaPolicy? GetPolicy(string priority)
    {
        if (!SlaCatalog.IsKnownPriority(priority))
        {
            return null;
        }

        var key = SlaCatalog.NormalizePriority(priority);
        using var db = factory.CreateDbContext();
        var row = db.Policies.AsNoTracking().FirstOrDefault(p => p.Priority == key);
        return row is null ? null : FromRow(row);
    }

    public void Upsert(SlaPolicy policy)
    {
        using var db = factory.CreateDbContext();
        var row = db.Policies.FirstOrDefault(p => p.Priority == policy.Priority);
        if (row is null)
        {
            db.Policies.Add(ToRow(policy));
        }
        else
        {
            row.FirstResponseMinutes = policy.FirstResponseMinutes;
            row.ResolutionMinutes = policy.ResolutionMinutes;
            row.UpdatedAt = policy.UpdatedAt;
        }

        db.SaveChanges();
    }

    private static IEnumerable<SlaPolicy> DefaultPolicies() =>
    [
        SlaPolicy.Create("Low", 480, 2880),
        SlaPolicy.Create("Medium", 240, 1440),
        SlaPolicy.Create("High", 60, 480),
        SlaPolicy.Create("Urgent", 15, 240)
    ];

    private static SlaPolicyRow ToRow(SlaPolicy policy) => new()
    {
        Priority = policy.Priority,
        FirstResponseMinutes = policy.FirstResponseMinutes,
        ResolutionMinutes = policy.ResolutionMinutes,
        UpdatedAt = policy.UpdatedAt
    };

    private static SlaPolicy FromRow(SlaPolicyRow row) =>
        SlaPolicy.Rehydrate(row.Priority, row.FirstResponseMinutes, row.ResolutionMinutes, row.UpdatedAt);
}
