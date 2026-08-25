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

    public IReadOnlyList<Ticket> Search(string? q, string? assignedAgentId)
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
            history);
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
            row.UpdatedAt);

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
}
