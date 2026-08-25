using Crm.Tickets.Api.Domain;
using Crm.Tickets.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Crm.Tickets.Api.Infrastructure;

/// <summary>SDD CRM-004 / CRM-037 — EF Core facade (same sync API as prior ADO store).</summary>
public sealed class TicketsDb(IDbContextFactory<TicketsDbContext> factory)
{
    public void EnsureSchema()
    {
        using var db = factory.CreateDbContext();
        db.Database.EnsureCreated();
        if (!db.TicketSequence.Any(s => s.Name == "ticket"))
        {
            db.TicketSequence.Add(new TicketSequenceRow { Name = "ticket", Value = 1000 });
            db.SaveChanges();
        }

        // EnsureCreated does not add new tables to an existing Sqlite file (CRM-016 / CRM-014).
        try
        {
            db.Database.ExecuteSqlRaw(
                """
                CREATE TABLE IF NOT EXISTS "TicketNotes" (
                    "Id" TEXT NOT NULL CONSTRAINT "PK_TicketNotes" PRIMARY KEY,
                    "TicketId" TEXT NOT NULL,
                    "Body" TEXT NOT NULL,
                    "AuthorName" TEXT NOT NULL,
                    "AuthorUserId" TEXT NULL,
                    "MentionedUserIdsJson" TEXT NOT NULL,
                    "CreatedAt" TEXT NOT NULL
                );
                CREATE INDEX IF NOT EXISTS "IX_TicketNotes_TicketId" ON "TicketNotes" ("TicketId");
                CREATE TABLE IF NOT EXISTS "TicketTasks" (
                    "Id" TEXT NOT NULL CONSTRAINT "PK_TicketTasks" PRIMARY KEY,
                    "TicketId" TEXT NOT NULL,
                    "Title" TEXT NOT NULL,
                    "DueAt" TEXT NULL,
                    "AssigneeUserId" TEXT NULL,
                    "AssigneeName" TEXT NULL,
                    "Status" TEXT NOT NULL,
                    "CreatedAt" TEXT NOT NULL,
                    "UpdatedAt" TEXT NOT NULL
                );
                CREATE INDEX IF NOT EXISTS "IX_TicketTasks_TicketId" ON "TicketTasks" ("TicketId");
                CREATE INDEX IF NOT EXISTS "IX_TicketTasks_AssigneeUserId" ON "TicketTasks" ("AssigneeUserId");
                CREATE TABLE IF NOT EXISTS "TicketFeedback" (
                    "Id" TEXT NOT NULL CONSTRAINT "PK_TicketFeedback" PRIMARY KEY,
                    "TicketId" TEXT NOT NULL,
                    "Rating" INTEGER NOT NULL,
                    "Comment" TEXT NULL,
                    "CreatedAt" TEXT NOT NULL
                );
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_TicketFeedback_TicketId" ON "TicketFeedback" ("TicketId");
                """);
            try
            {
                db.Database.ExecuteSqlRaw("""ALTER TABLE "Tickets" ADD COLUMN "DepartmentId" TEXT NULL;""");
            }
            catch
            {
                // column may exist
            }
        }
        catch
        {
            // SQL Server / already migrated — EF model covers new DBs
        }

        try
        {
            using var dbSql = factory.CreateDbContext();
            if (!dbSql.Database.IsSqlite())
            {
                dbSql.Database.ExecuteSqlRaw(
                    """
                    IF COL_LENGTH('Tickets', 'DepartmentId') IS NULL
                      ALTER TABLE [Tickets] ADD [DepartmentId] uniqueidentifier NULL;
                    """);
            }
        }
        catch
        {
            // ignore
        }
    }

    public void SeedIfEmpty()
    {
        try
        {
            using (var db = factory.CreateDbContext())
            {
                if (db.Tickets.Any())
                {
                    return;
                }
            }

            var demoCustomerId = Guid.Parse("0a50ed5a-1796-4555-bc08-5b28cf59a539");
            var ticket = Ticket.Create(
                NextTicketNumber(),
                demoCustomerId,
                "Acme Industries",
                "Invoice mismatch on March statement",
                "Customer reports line items do not match PO.",
                "Billing",
                "High",
                "Demo Agent");
            ticket.Assign(TicketCatalog.Agents[0].Id, TicketCatalog.Agents[0].Name, "Demo Agent");
            Insert(ticket);
            InsertTask(TicketTask.Create(
                ticket.Id,
                "Call Acme AP about PO screenshots",
                DateTimeOffset.UtcNow.Date.AddDays(1),
                TicketCatalog.Agents[0].Id,
                TicketCatalog.Agents[0].Name));

            var open = Ticket.Create(
                NextTicketNumber(),
                Guid.Parse("72f13bfd-ef8f-45b0-b7e0-57b3d353a70b"),
                "Beta LLC",
                "Cannot reset WhatsApp channel token",
                null,
                "Technical",
                "Urgent",
                "System");
            Insert(open);
        }
        catch
        {
            // never brick startup
        }
    }

    public string NextTicketNumber()
    {
        using var db = factory.CreateDbContext();
        var seq = db.TicketSequence.FirstOrDefault(s => s.Name == "ticket");
        if (seq is null)
        {
            seq = new TicketSequenceRow { Name = "ticket", Value = 1000 };
            db.TicketSequence.Add(seq);
        }

        seq.Value += 1;
        db.SaveChanges();
        return $"TKT-{seq.Value}";
    }

    public void Insert(Ticket ticket)
    {
        using var db = factory.CreateDbContext();
        db.Tickets.Add(ToRow(ticket));
        foreach (var entry in ticket.History)
        {
            db.TicketHistory.Add(ToRow(entry));
        }

        db.SaveChanges();
    }

    public void Update(Ticket ticket)
    {
        using var db = factory.CreateDbContext();
        var row = db.Tickets.FirstOrDefault(t => t.Id == ticket.Id)
                  ?? throw new InvalidOperationException("Ticket not found.");
        row.CustomerName = ticket.CustomerName;
        row.Subject = ticket.Subject;
        row.Description = ticket.Description;
        row.Category = ticket.Category;
        row.Priority = ticket.Priority;
        row.Status = ticket.Status;
        row.AssignedAgentId = ticket.AssignedAgentId;
        row.AssignedAgentName = ticket.AssignedAgentName;
        row.IsEscalated = ticket.IsEscalated;
        row.DepartmentId = ticket.DepartmentId;
        row.UpdatedAt = ticket.UpdatedAt;

        var existingIds = db.TicketHistory.Where(h => h.TicketId == ticket.Id).Select(h => h.Id).ToHashSet();
        foreach (var entry in ticket.History)
        {
            if (existingIds.Contains(entry.Id))
            {
                continue;
            }

            db.TicketHistory.Add(ToRow(entry));
        }

        db.SaveChanges();
    }

    public IReadOnlyList<Ticket> Search(string? q, string? assignedAgentId, Guid? departmentId = null)
    {
        using var db = factory.CreateDbContext();
        IQueryable<TicketRow> query = db.Tickets.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            query = query.Where(t =>
                t.TicketNumber.ToLower().Contains(term)
                || t.CustomerName.ToLower().Contains(term)
                || t.Subject.ToLower().Contains(term)
                || t.CustomerId.ToString().ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(assignedAgentId))
        {
            var aid = assignedAgentId.Trim();
            query = query.Where(t => t.AssignedAgentId == aid);
        }

        if (departmentId is { } dept)
        {
            query = query.Where(t => t.DepartmentId == dept);
        }

        return query
            .Take(200)
            .ToList()
            .OrderByDescending(t => t.UpdatedAt)
            .Take(100)
            .Select(FromRow)
            .ToList();
    }

    public Ticket? Get(Guid id)
    {
        using var db = factory.CreateDbContext();
        var row = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == id);
        if (row is null)
        {
            return null;
        }

        var history = db.TicketHistory.AsNoTracking()
            .Where(h => h.TicketId == id)
            .ToList()
            .OrderBy(h => h.ChangedAt)
            .Select(h => TicketHistoryEntry.Rehydrate(
                h.Id, h.TicketId, h.Field, h.OldValue, h.NewValue, h.ChangedBy, h.ChangedAt));

        return Ticket.Rehydrate(
            row.Id,
            row.TicketNumber,
            row.CustomerId,
            row.CustomerName,
            row.Subject,
            row.Description,
            row.Category,
            row.Priority,
            row.Status,
            row.AssignedAgentId,
            row.AssignedAgentName,
            row.IsEscalated,
            row.CreatedAt,
            row.UpdatedAt,
            history,
            row.DepartmentId);
    }

    /// <summary>SDD CRM-030 — lookup by ticket number (portal feedback).</summary>
    public Ticket? GetByTicketNumber(string ticketNumber)
    {
        if (string.IsNullOrWhiteSpace(ticketNumber))
        {
            return null;
        }

        using var db = factory.CreateDbContext();
        var num = ticketNumber.Trim().ToLower();
        var row = db.Tickets.AsNoTracking()
            .FirstOrDefault(t => t.TicketNumber.ToLower() == num);
        return row is null ? null : Get(row.Id);
    }

    /// <summary>SDD CRM-016 — list internal notes for a ticket (newest first).</summary>
    public IReadOnlyList<TicketNote> ListNotes(Guid ticketId)
    {
        using var db = factory.CreateDbContext();
        return db.TicketNotes.AsNoTracking()
            .Where(n => n.TicketId == ticketId)
            .ToList()
            .OrderByDescending(n => n.CreatedAt)
            .Select(FromNoteRow)
            .ToList();
    }

    /// <summary>SDD CRM-016 — persist an internal collaboration note.</summary>
    public void InsertNote(TicketNote note)
    {
        using var db = factory.CreateDbContext();
        db.TicketNotes.Add(ToNoteRow(note));
        db.SaveChanges();
    }

    /// <summary>SDD CRM-014 — list tasks for a ticket.</summary>
    public IReadOnlyList<TicketTask> ListTasks(Guid ticketId)
    {
        using var db = factory.CreateDbContext();
        return db.TicketTasks.AsNoTracking()
            .Where(t => t.TicketId == ticketId)
            .ToList()
            .OrderBy(t => t.Status == "Open" ? 0 : 1)
            .ThenBy(t => t.DueAt ?? DateTimeOffset.MaxValue)
            .Select(FromTaskRow)
            .ToList();
    }

    /// <summary>SDD CRM-014 — open tasks for an assignee (optional due filter).</summary>
    public IReadOnlyList<TicketTask> ListOpenTasks(string? assigneeUserId, DateTimeOffset? dueOnOrBefore)
    {
        using var db = factory.CreateDbContext();
        IEnumerable<TicketTaskRow> rows = db.TicketTasks.AsNoTracking()
            .Where(t => t.Status == "Open")
            .ToList();
        if (!string.IsNullOrWhiteSpace(assigneeUserId))
        {
            var aid = assigneeUserId.Trim();
            rows = rows.Where(t =>
                t.AssigneeUserId != null &&
                t.AssigneeUserId.Equals(aid, StringComparison.OrdinalIgnoreCase));
        }

        if (dueOnOrBefore is not null)
        {
            var until = dueOnOrBefore.Value;
            rows = rows.Where(t => t.DueAt is not null && t.DueAt <= until);
        }

        return rows.OrderBy(t => t.DueAt ?? DateTimeOffset.MaxValue).Select(FromTaskRow).ToList();
    }

    public void InsertTask(TicketTask task)
    {
        using var db = factory.CreateDbContext();
        db.TicketTasks.Add(ToTaskRow(task));
        db.SaveChanges();
    }

    public TicketTask? GetTask(Guid taskId)
    {
        using var db = factory.CreateDbContext();
        var row = db.TicketTasks.AsNoTracking().FirstOrDefault(t => t.Id == taskId);
        return row is null ? null : FromTaskRow(row);
    }

    public void UpdateTask(TicketTask task)
    {
        using var db = factory.CreateDbContext();
        var row = db.TicketTasks.FirstOrDefault(t => t.Id == task.Id)
                  ?? throw new InvalidOperationException("Task not found.");
        row.Title = task.Title;
        row.DueAt = task.DueAt;
        row.AssigneeUserId = task.AssigneeUserId;
        row.AssigneeName = task.AssigneeName;
        row.Status = task.Status;
        row.UpdatedAt = task.UpdatedAt;
        db.SaveChanges();
    }

    /// <summary>SDD CRM-030</summary>
    public TicketFeedback? GetFeedback(Guid ticketId)
    {
        using var db = factory.CreateDbContext();
        var row = db.TicketFeedback.AsNoTracking().FirstOrDefault(f => f.TicketId == ticketId);
        return row is null ? null : FromFeedbackRow(row);
    }

    /// <summary>SDD CRM-030</summary>
    public void InsertFeedback(TicketFeedback feedback)
    {
        using var db = factory.CreateDbContext();
        if (db.TicketFeedback.Any(f => f.TicketId == feedback.TicketId))
        {
            throw new InvalidOperationException("Feedback already exists for this ticket.");
        }

        db.TicketFeedback.Add(ToFeedbackRow(feedback));
        db.SaveChanges();
    }

    /// <summary>SDD CRM-031 / 032 — tickets created in inclusive range (report snapshot).</summary>
    public IReadOnlyList<TicketReportSnapshot> ListTicketsCreatedBetween(DateTimeOffset from, DateTimeOffset to)
    {
        using var db = factory.CreateDbContext();
        return db.Tickets.AsNoTracking()
            .ToList()
            .Where(t => t.CreatedAt >= from && t.CreatedAt <= to)
            .Select(t => new TicketReportSnapshot(
                t.Id,
                t.Status,
                t.Category,
                t.Priority,
                t.AssignedAgentId,
                t.AssignedAgentName,
                t.IsEscalated,
                t.CreatedAt,
                t.UpdatedAt))
            .ToList();
    }

    /// <summary>SDD CRM-033 — feedback created in range with ticket assignee.</summary>
    public IReadOnlyList<FeedbackReportSnapshot> ListFeedbackForReport(DateTimeOffset from, DateTimeOffset to)
    {
        using var db = factory.CreateDbContext();
        var feedback = db.TicketFeedback.AsNoTracking()
            .ToList()
            .Where(f => f.CreatedAt >= from && f.CreatedAt <= to)
            .ToList();
        var ticketIds = feedback.Select(f => f.TicketId).Distinct().ToHashSet();
        var tickets = db.Tickets.AsNoTracking()
            .ToList()
            .Where(t => ticketIds.Contains(t.Id))
            .ToDictionary(t => t.Id);
        return feedback
            .Select(f =>
            {
                tickets.TryGetValue(f.TicketId, out var t);
                return new FeedbackReportSnapshot(
                    f.Rating,
                    t?.AssignedAgentId,
                    t?.AssignedAgentName);
            })
            .ToList();
    }

    private static Ticket FromRow(TicketRow row)
        => Ticket.Rehydrate(
            row.Id,
            row.TicketNumber,
            row.CustomerId,
            row.CustomerName,
            row.Subject,
            row.Description,
            row.Category,
            row.Priority,
            row.Status,
            row.AssignedAgentId,
            row.AssignedAgentName,
            row.IsEscalated,
            row.CreatedAt,
            row.UpdatedAt,
            departmentId: row.DepartmentId);

    private static TicketRow ToRow(Ticket t) => new()
    {
        Id = t.Id,
        TicketNumber = t.TicketNumber,
        CustomerId = t.CustomerId,
        CustomerName = t.CustomerName,
        Subject = t.Subject,
        Description = t.Description,
        Category = t.Category,
        Priority = t.Priority,
        Status = t.Status,
        AssignedAgentId = t.AssignedAgentId,
        AssignedAgentName = t.AssignedAgentName,
        IsEscalated = t.IsEscalated,
        DepartmentId = t.DepartmentId,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt
    };

    private static TicketHistoryRow ToRow(TicketHistoryEntry e) => new()
    {
        Id = e.Id,
        TicketId = e.TicketId,
        Field = e.Field,
        OldValue = e.OldValue,
        NewValue = e.NewValue,
        ChangedBy = e.ChangedBy,
        ChangedAt = e.ChangedAt
    };

    private static TicketNoteRow ToNoteRow(TicketNote n) => new()
    {
        Id = n.Id,
        TicketId = n.TicketId,
        Body = n.Body,
        AuthorName = n.AuthorName,
        AuthorUserId = n.AuthorUserId,
        MentionedUserIdsJson = System.Text.Json.JsonSerializer.Serialize(n.MentionedUserIds),
        CreatedAt = n.CreatedAt
    };

    private static TicketNote FromNoteRow(TicketNoteRow row)
    {
        var mentions = Array.Empty<string>();
        try
        {
            mentions = System.Text.Json.JsonSerializer.Deserialize<string[]>(row.MentionedUserIdsJson) ?? [];
        }
        catch
        {
            // ignore corrupt json
        }

        return TicketNote.Rehydrate(
            row.Id,
            row.TicketId,
            row.Body,
            row.AuthorName,
            row.AuthorUserId,
            mentions,
            row.CreatedAt);
    }

    private static TicketTaskRow ToTaskRow(TicketTask t) => new()
    {
        Id = t.Id,
        TicketId = t.TicketId,
        Title = t.Title,
        DueAt = t.DueAt,
        AssigneeUserId = t.AssigneeUserId,
        AssigneeName = t.AssigneeName,
        Status = t.Status,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt
    };

    private static TicketTask FromTaskRow(TicketTaskRow row) =>
        TicketTask.Rehydrate(
            row.Id,
            row.TicketId,
            row.Title,
            row.DueAt,
            row.AssigneeUserId,
            row.AssigneeName,
            row.Status,
            row.CreatedAt,
            row.UpdatedAt);

    private static TicketFeedbackRow ToFeedbackRow(TicketFeedback f) => new()
    {
        Id = f.Id,
        TicketId = f.TicketId,
        Rating = f.Rating,
        Comment = f.Comment,
        CreatedAt = f.CreatedAt
    };

    private static TicketFeedback FromFeedbackRow(TicketFeedbackRow row) =>
        TicketFeedback.Rehydrate(row.Id, row.TicketId, row.Rating, row.Comment, row.CreatedAt);
}

/// <summary>SDD CRM-031 / CRM-032 — lightweight ticket row for reports.</summary>
public sealed record TicketReportSnapshot(
    Guid Id,
    string Status,
    string Category,
    string Priority,
    string? AssignedAgentId,
    string? AssignedAgentName,
    bool IsEscalated,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

/// <summary>SDD CRM-033 — feedback + assignee for CSAT reports.</summary>
public sealed record FeedbackReportSnapshot(
    int Rating,
    string? AssignedAgentId,
    string? AssignedAgentName);
