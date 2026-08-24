using Crm.Tickets.Api.Domain;
using Microsoft.Data.Sqlite;

namespace Crm.Tickets.Api.Infrastructure;

/// <summary>SDD CRM-004 / specs/003-ticket-lifecycle — SQLite persistence.</summary>
public sealed class TicketsDb
{
    private readonly string _connectionString;
    private long _sequence;

    public TicketsDb(IWebHostEnvironment env, IConfiguration config)
    {
        var dataRoot = Path.GetFullPath(config["Tickets:DataPath"] ?? Path.Combine(env.ContentRootPath, "data"));
        Directory.CreateDirectory(dataRoot);
        _connectionString = $"Data Source={Path.Combine(dataRoot, "tickets.db")}";
    }

    public void EnsureSchema()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Tickets (
              Id TEXT PRIMARY KEY,
              TicketNumber TEXT NOT NULL UNIQUE,
              CustomerId TEXT NOT NULL,
              CustomerName TEXT NOT NULL,
              Subject TEXT NOT NULL,
              Description TEXT NULL,
              Category TEXT NOT NULL,
              Priority TEXT NOT NULL,
              Status TEXT NOT NULL,
              AssignedAgentId TEXT NULL,
              AssignedAgentName TEXT NULL,
              IsEscalated INTEGER NOT NULL,
              CreatedAt TEXT NOT NULL,
              UpdatedAt TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS TicketHistory (
              Id TEXT PRIMARY KEY,
              TicketId TEXT NOT NULL,
              Field TEXT NOT NULL,
              OldValue TEXT NULL,
              NewValue TEXT NULL,
              ChangedBy TEXT NOT NULL,
              ChangedAt TEXT NOT NULL,
              FOREIGN KEY (TicketId) REFERENCES Tickets(Id)
            );
            CREATE TABLE IF NOT EXISTS TicketSequence (
              Name TEXT PRIMARY KEY,
              Value INTEGER NOT NULL
            );
            INSERT OR IGNORE INTO TicketSequence (Name, Value) VALUES ('ticket', 1000);
            """;
        command.ExecuteNonQuery();
        using var seq = connection.CreateCommand();
        seq.CommandText = "SELECT Value FROM TicketSequence WHERE Name = 'ticket'";
        _sequence = Convert.ToInt64(seq.ExecuteScalar());
    }

    public void SeedIfEmpty()
    {
        try
        {
            using var connection = Open();
            using var countCmd = connection.CreateCommand();
            countCmd.CommandText = "SELECT COUNT(1) FROM Tickets";
            if (Convert.ToInt64(countCmd.ExecuteScalar()) > 0)
            {
                return;
            }

            var demoCustomerId = Guid.Parse("0a50ed5a-1796-4555-bc08-5b28cf59a539");
            // fallback stable seed ids if Acme not present in Customers DB
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
        using var connection = Open();
        using var tx = connection.BeginTransaction();
        using (var update = connection.CreateCommand())
        {
            update.Transaction = tx;
            update.CommandText = "UPDATE TicketSequence SET Value = Value + 1 WHERE Name = 'ticket'";
            update.ExecuteNonQuery();
        }

        using var read = connection.CreateCommand();
        read.Transaction = tx;
        read.CommandText = "SELECT Value FROM TicketSequence WHERE Name = 'ticket'";
        _sequence = Convert.ToInt64(read.ExecuteScalar());
        tx.Commit();
        return $"TKT-{_sequence}";
    }

    public void Insert(Ticket ticket)
    {
        using var connection = Open();
        using var tx = connection.BeginTransaction();
        using (var command = connection.CreateCommand())
        {
            command.Transaction = tx;
            command.CommandText =
                """
                INSERT INTO Tickets
                (Id, TicketNumber, CustomerId, CustomerName, Subject, Description, Category, Priority, Status,
                 AssignedAgentId, AssignedAgentName, IsEscalated, CreatedAt, UpdatedAt)
                VALUES
                ($id, $num, $cid, $cname, $subject, $desc, $cat, $pri, $status, $aid, $aname, $esc, $created, $updated)
                """;
            BindTicket(command, ticket);
            command.ExecuteNonQuery();
        }

        foreach (var entry in ticket.History)
        {
            InsertHistory(connection, tx, entry);
        }

        tx.Commit();
    }

    public void Update(Ticket ticket)
    {
        using var connection = Open();
        using var tx = connection.BeginTransaction();
        using (var command = connection.CreateCommand())
        {
            command.Transaction = tx;
            command.CommandText =
                """
                UPDATE Tickets SET
                  CustomerName = $cname, Subject = $subject, Description = $desc, Category = $cat, Priority = $pri,
                  Status = $status, AssignedAgentId = $aid, AssignedAgentName = $aname, IsEscalated = $esc, UpdatedAt = $updated
                WHERE Id = $id
                """;
            command.Parameters.AddWithValue("$id", ticket.Id.ToString());
            command.Parameters.AddWithValue("$cname", ticket.CustomerName);
            command.Parameters.AddWithValue("$subject", ticket.Subject);
            command.Parameters.AddWithValue("$desc", (object?)ticket.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("$cat", ticket.Category);
            command.Parameters.AddWithValue("$pri", ticket.Priority);
            command.Parameters.AddWithValue("$status", ticket.Status);
            command.Parameters.AddWithValue("$aid", (object?)ticket.AssignedAgentId ?? DBNull.Value);
            command.Parameters.AddWithValue("$aname", (object?)ticket.AssignedAgentName ?? DBNull.Value);
            command.Parameters.AddWithValue("$esc", ticket.IsEscalated ? 1 : 0);
            command.Parameters.AddWithValue("$updated", ticket.UpdatedAt.ToString("O"));
            command.ExecuteNonQuery();
        }

        foreach (var entry in ticket.History)
        {
            InsertHistoryIgnore(connection, tx, entry);
        }

        tx.Commit();
    }

    private static void InsertHistoryIgnore(SqliteConnection connection, SqliteTransaction tx, TicketHistoryEntry entry)
    {
        using var command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText =
            """
            INSERT OR IGNORE INTO TicketHistory (Id, TicketId, Field, OldValue, NewValue, ChangedBy, ChangedAt)
            VALUES ($id, $tid, $field, $old, $new, $by, $at)
            """;
        command.Parameters.AddWithValue("$id", entry.Id.ToString());
        command.Parameters.AddWithValue("$tid", entry.TicketId.ToString());
        command.Parameters.AddWithValue("$field", entry.Field);
        command.Parameters.AddWithValue("$old", (object?)entry.OldValue ?? DBNull.Value);
        command.Parameters.AddWithValue("$new", (object?)entry.NewValue ?? DBNull.Value);
        command.Parameters.AddWithValue("$by", entry.ChangedBy);
        command.Parameters.AddWithValue("$at", entry.ChangedAt.ToString("O"));
        command.ExecuteNonQuery();
    }

    public IReadOnlyList<Ticket> Search(string? q, string? assignedAgentId)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        var clauses = new List<string>();
        if (!string.IsNullOrWhiteSpace(q))
        {
            clauses.Add("(TicketNumber LIKE $q OR CustomerName LIKE $q OR Subject LIKE $q OR CustomerId LIKE $q)");
            command.Parameters.AddWithValue("$q", $"%{q.Trim()}%");
        }

        if (!string.IsNullOrWhiteSpace(assignedAgentId))
        {
            clauses.Add("AssignedAgentId = $aid");
            command.Parameters.AddWithValue("$aid", assignedAgentId.Trim());
        }

        var where = clauses.Count == 0 ? "" : "WHERE " + string.Join(" AND ", clauses);
        command.CommandText =
            $"""
             SELECT Id, TicketNumber, CustomerId, CustomerName, Subject, Description, Category, Priority, Status,
                    AssignedAgentId, AssignedAgentName, IsEscalated, CreatedAt, UpdatedAt
             FROM Tickets
             {where}
             ORDER BY UpdatedAt DESC
             LIMIT 100
             """;
        var list = new List<Ticket>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(ReadTicket(reader, includeHistory: false));
        }

        return list;
    }

    public Ticket? Get(Guid id)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, TicketNumber, CustomerId, CustomerName, Subject, Description, Category, Priority, Status,
                   AssignedAgentId, AssignedAgentName, IsEscalated, CreatedAt, UpdatedAt
            FROM Tickets WHERE Id = $id
            """;
        command.Parameters.AddWithValue("$id", id.ToString());
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var ticket = ReadTicket(reader, includeHistory: false);
        reader.Close();
        return Ticket.Rehydrate(
            ticket.Id,
            ticket.TicketNumber,
            ticket.CustomerId,
            ticket.CustomerName,
            ticket.Subject,
            ticket.Description,
            ticket.Category,
            ticket.Priority,
            ticket.Status,
            ticket.AssignedAgentId,
            ticket.AssignedAgentName,
            ticket.IsEscalated,
            ticket.CreatedAt,
            ticket.UpdatedAt,
            LoadHistory(connection, ticket.Id));
    }

    private static void BindTicket(SqliteCommand command, Ticket ticket)
    {
        command.Parameters.AddWithValue("$id", ticket.Id.ToString());
        command.Parameters.AddWithValue("$num", ticket.TicketNumber);
        command.Parameters.AddWithValue("$cid", ticket.CustomerId.ToString());
        command.Parameters.AddWithValue("$cname", ticket.CustomerName);
        command.Parameters.AddWithValue("$subject", ticket.Subject);
        command.Parameters.AddWithValue("$desc", (object?)ticket.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("$cat", ticket.Category);
        command.Parameters.AddWithValue("$pri", ticket.Priority);
        command.Parameters.AddWithValue("$status", ticket.Status);
        command.Parameters.AddWithValue("$aid", (object?)ticket.AssignedAgentId ?? DBNull.Value);
        command.Parameters.AddWithValue("$aname", (object?)ticket.AssignedAgentName ?? DBNull.Value);
        command.Parameters.AddWithValue("$esc", ticket.IsEscalated ? 1 : 0);
        command.Parameters.AddWithValue("$created", ticket.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("$updated", ticket.UpdatedAt.ToString("O"));
    }

    private static void InsertHistory(SqliteConnection connection, SqliteTransaction tx, TicketHistoryEntry entry)
    {
        using var command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText =
            """
            INSERT INTO TicketHistory (Id, TicketId, Field, OldValue, NewValue, ChangedBy, ChangedAt)
            VALUES ($id, $tid, $field, $old, $new, $by, $at)
            """;
        command.Parameters.AddWithValue("$id", entry.Id.ToString());
        command.Parameters.AddWithValue("$tid", entry.TicketId.ToString());
        command.Parameters.AddWithValue("$field", entry.Field);
        command.Parameters.AddWithValue("$old", (object?)entry.OldValue ?? DBNull.Value);
        command.Parameters.AddWithValue("$new", (object?)entry.NewValue ?? DBNull.Value);
        command.Parameters.AddWithValue("$by", entry.ChangedBy);
        command.Parameters.AddWithValue("$at", entry.ChangedAt.ToString("O"));
        command.ExecuteNonQuery();
    }

    private static List<TicketHistoryEntry> LoadHistory(SqliteConnection connection, Guid ticketId)
    {
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, TicketId, Field, OldValue, NewValue, ChangedBy, ChangedAt
            FROM TicketHistory WHERE TicketId = $tid ORDER BY ChangedAt
            """;
        command.Parameters.AddWithValue("$tid", ticketId.ToString());
        var list = new List<TicketHistoryEntry>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(TicketHistoryEntry.Rehydrate(
                Guid.Parse(reader.GetString(0)),
                Guid.Parse(reader.GetString(1)),
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.GetString(5),
                DateTimeOffset.Parse(reader.GetString(6))));
        }

        return list;
    }

    private static Ticket ReadTicket(SqliteDataReader reader, bool includeHistory)
    {
        return Ticket.Rehydrate(
            Guid.Parse(reader.GetString(0)),
            reader.GetString(1),
            Guid.Parse(reader.GetString(2)),
            reader.GetString(3),
            reader.GetString(4),
            reader.IsDBNull(5) ? null : reader.GetString(5),
            reader.GetString(6),
            reader.GetString(7),
            reader.GetString(8),
            reader.IsDBNull(9) ? null : reader.GetString(9),
            reader.IsDBNull(10) ? null : reader.GetString(10),
            reader.GetInt64(11) == 1,
            DateTimeOffset.Parse(reader.GetString(12)),
            DateTimeOffset.Parse(reader.GetString(13)));
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
