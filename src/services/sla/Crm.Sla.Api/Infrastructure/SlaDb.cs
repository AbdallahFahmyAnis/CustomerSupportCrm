using Crm.Sla.Api.Domain;
using Crm.Sla.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Crm.Sla.Api.Infrastructure;

/// <summary>SDD CRM-017 / CRM-018 / CRM-019 — EF Core facade for SLA.</summary>
public sealed class SlaDb(IDbContextFactory<SlaDbContext> factory)
{
    public void EnsureSchema()
    {
        using var db = factory.CreateDbContext();
        db.Database.EnsureCreated();
        EnsureAutomationTables(db);
    }

    public void SeedIfEmpty()
    {
        try
        {
            SeedPoliciesIfEmpty();
            SeedAssignRulesIfEmpty();
            SeedEscalationIfEmpty();
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
            .Select(FromPolicyRow)
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
        return row is null ? null : FromPolicyRow(row);
    }

    public void Upsert(SlaPolicy policy)
    {
        using var db = factory.CreateDbContext();
        var row = db.Policies.FirstOrDefault(p => p.Priority == policy.Priority);
        if (row is null)
        {
            db.Policies.Add(ToPolicyRow(policy));
        }
        else
        {
            row.FirstResponseMinutes = policy.FirstResponseMinutes;
            row.ResolutionMinutes = policy.ResolutionMinutes;
            row.UpdatedAt = policy.UpdatedAt;
        }

        db.SaveChanges();
    }

    public IReadOnlyList<AutoAssignRule> ListAssignRules()
    {
        using var db = factory.CreateDbContext();
        return db.AssignRules.AsNoTracking().ToList()
            .OrderByDescending(r => (r.Category == null ? 0 : 2) + (r.Priority == null ? 0 : 1))
            .Select(FromAssignRow)
            .ToList();
    }

    public void ReplaceAssignRules(IReadOnlyList<AutoAssignRule> rules)
    {
        using var db = factory.CreateDbContext();
        db.AssignRules.RemoveRange(db.AssignRules);
        foreach (var rule in rules)
        {
            db.AssignRules.Add(ToAssignRow(rule));
        }

        db.SaveChanges();
    }

    public EscalationSettings GetEscalationSettings()
    {
        using var db = factory.CreateDbContext();
        var row = db.EscalationSettings.AsNoTracking()
            .FirstOrDefault(x => x.Id == EscalationSettings.SingletonId);
        return row is null ? EscalationSettings.CreateDefault() : FromEscalationRow(row);
    }

    public void SaveEscalationSettings(EscalationSettings settings)
    {
        using var db = factory.CreateDbContext();
        var row = db.EscalationSettings.FirstOrDefault(x => x.Id == EscalationSettings.SingletonId);
        if (row is null)
        {
            db.EscalationSettings.Add(ToEscalationRow(settings));
        }
        else
        {
            row.EscalateOnFirstResponseBreach = settings.EscalateOnFirstResponseBreach;
            row.EscalateOnResolutionBreach = settings.EscalateOnResolutionBreach;
            row.EscalateUrgentAlways = settings.EscalateUrgentAlways;
            row.AssignToAgentId = settings.AssignToAgentId;
            row.AssignToAgentName = settings.AssignToAgentName;
            row.UpdatedAt = settings.UpdatedAt;
        }

        db.SaveChanges();
    }

    private void SeedPoliciesIfEmpty()
    {
        using var db = factory.CreateDbContext();
        if (db.Policies.Any())
        {
            return;
        }

        foreach (var policy in DefaultPolicies())
        {
            db.Policies.Add(ToPolicyRow(policy));
        }

        db.SaveChanges();
    }

    private void SeedAssignRulesIfEmpty()
    {
        using var db = factory.CreateDbContext();
        if (db.AssignRules.Any())
        {
            return;
        }

        foreach (var rule in DefaultAssignRules())
        {
            db.AssignRules.Add(ToAssignRow(rule));
        }

        db.SaveChanges();
    }

    private void SeedEscalationIfEmpty()
    {
        using var db = factory.CreateDbContext();
        if (db.EscalationSettings.Any())
        {
            return;
        }

        db.EscalationSettings.Add(ToEscalationRow(EscalationSettings.CreateDefault()));
        db.SaveChanges();
    }

    private static void EnsureAutomationTables(SlaDbContext db)
    {
        try
        {
            // EnsureCreated no-ops when the DB already exists from CRM-017 — add tables if missing (Sqlite).
            db.Database.ExecuteSqlRaw(
                """
                CREATE TABLE IF NOT EXISTS "AutoAssignRules" (
                    "Id" TEXT NOT NULL CONSTRAINT "PK_AutoAssignRules" PRIMARY KEY,
                    "Category" TEXT NULL,
                    "Priority" TEXT NULL,
                    "AgentId" TEXT NOT NULL,
                    "AgentName" TEXT NOT NULL,
                    "Enabled" INTEGER NOT NULL
                );
                """);
            db.Database.ExecuteSqlRaw(
                """
                CREATE TABLE IF NOT EXISTS "EscalationSettings" (
                    "Id" TEXT NOT NULL CONSTRAINT "PK_EscalationSettings" PRIMARY KEY,
                    "EscalateOnFirstResponseBreach" INTEGER NOT NULL,
                    "EscalateOnResolutionBreach" INTEGER NOT NULL,
                    "EscalateUrgentAlways" INTEGER NOT NULL,
                    "AssignToAgentId" TEXT NOT NULL,
                    "AssignToAgentName" TEXT NOT NULL,
                    "UpdatedAt" TEXT NOT NULL
                );
                """);
        }
        catch
        {
            // SQL Server / fresh EnsureCreated already has tables
        }
    }

    private static IEnumerable<SlaPolicy> DefaultPolicies() =>
    [
        SlaPolicy.Create("Low", 480, 2880),
        SlaPolicy.Create("Medium", 240, 1440),
        SlaPolicy.Create("High", 60, 480),
        SlaPolicy.Create("Urgent", 15, 240)
    ];

    private static IEnumerable<AutoAssignRule> DefaultAssignRules() =>
    [
        AutoAssignRule.Create("Technical", null, "11111111-1111-1111-1111-111111111111", "Demo Agent"),
        AutoAssignRule.Create(null, "Urgent", "22222222-2222-2222-2222-222222222222", "Lead Agent"),
        AutoAssignRule.Create(null, "High", "22222222-2222-2222-2222-222222222222", "Lead Agent"),
        AutoAssignRule.Create(null, null, "11111111-1111-1111-1111-111111111111", "Demo Agent")
    ];

    private static SlaPolicyRow ToPolicyRow(SlaPolicy policy) => new()
    {
        Priority = policy.Priority,
        FirstResponseMinutes = policy.FirstResponseMinutes,
        ResolutionMinutes = policy.ResolutionMinutes,
        UpdatedAt = policy.UpdatedAt
    };

    private static SlaPolicy FromPolicyRow(SlaPolicyRow row) =>
        SlaPolicy.Rehydrate(row.Priority, row.FirstResponseMinutes, row.ResolutionMinutes, row.UpdatedAt);

    private static AutoAssignRuleRow ToAssignRow(AutoAssignRule rule) => new()
    {
        Id = rule.Id,
        Category = rule.Category,
        Priority = rule.Priority,
        AgentId = rule.AgentId,
        AgentName = rule.AgentName,
        Enabled = rule.Enabled
    };

    private static AutoAssignRule FromAssignRow(AutoAssignRuleRow row) =>
        AutoAssignRule.Create(row.Category, row.Priority, row.AgentId, row.AgentName, row.Enabled, row.Id);

    private static EscalationSettingsRow ToEscalationRow(EscalationSettings settings) => new()
    {
        Id = EscalationSettings.SingletonId,
        EscalateOnFirstResponseBreach = settings.EscalateOnFirstResponseBreach,
        EscalateOnResolutionBreach = settings.EscalateOnResolutionBreach,
        EscalateUrgentAlways = settings.EscalateUrgentAlways,
        AssignToAgentId = settings.AssignToAgentId,
        AssignToAgentName = settings.AssignToAgentName,
        UpdatedAt = settings.UpdatedAt
    };

    private static EscalationSettings FromEscalationRow(EscalationSettingsRow row) =>
        EscalationSettings.Rehydrate(
            row.EscalateOnFirstResponseBreach,
            row.EscalateOnResolutionBreach,
            row.EscalateUrgentAlways,
            row.AssignToAgentId,
            row.AssignToAgentName,
            row.UpdatedAt);
}
