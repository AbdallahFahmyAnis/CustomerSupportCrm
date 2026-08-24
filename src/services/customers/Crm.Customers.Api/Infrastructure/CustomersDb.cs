using Crm.Customers.Api.Domain;
using Microsoft.Data.Sqlite;

namespace Crm.Customers.Api.Infrastructure;

/// <summary>SDD CRM-001 / specs/002-customer-profiles — SQLite persistence for Customers.</summary>
public sealed class CustomersDb
{
    private readonly string _connectionString;
    private readonly string _dataRoot;
    private readonly string _attachmentsRoot;

    public CustomersDb(IWebHostEnvironment env, IConfiguration config)
    {
        _dataRoot = Path.GetFullPath(config["Customers:DataPath"] ?? Path.Combine(env.ContentRootPath, "data"));
        Directory.CreateDirectory(_dataRoot);
        _attachmentsRoot = Path.Combine(_dataRoot, "attachments");
        Directory.CreateDirectory(_attachmentsRoot);
        var dbPath = Path.Combine(_dataRoot, "customers.db");
        _connectionString = $"Data Source={dbPath}";
    }

    public string AttachmentsRoot => _attachmentsRoot;

    public void EnsureSchema()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Customers (
              Id TEXT PRIMARY KEY,
              DisplayName TEXT NOT NULL,
              UniqueIdentifier TEXT NOT NULL COLLATE NOCASE,
              Organization TEXT NULL,
              Status TEXT NOT NULL,
              CreatedAt TEXT NOT NULL,
              UpdatedAt TEXT NOT NULL
            );
            CREATE UNIQUE INDEX IF NOT EXISTS IX_Customers_UniqueIdentifier ON Customers(UniqueIdentifier);
            CREATE TABLE IF NOT EXISTS Contacts (
              Id TEXT PRIMARY KEY,
              CustomerId TEXT NOT NULL,
              Type TEXT NOT NULL,
              Value TEXT NOT NULL,
              IsPrimary INTEGER NOT NULL,
              IsActive INTEGER NOT NULL,
              CreatedAt TEXT NOT NULL,
              FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
            );
            CREATE TABLE IF NOT EXISTS Notes (
              Id TEXT PRIMARY KEY,
              CustomerId TEXT NOT NULL,
              Body TEXT NOT NULL,
              AuthorName TEXT NOT NULL,
              CreatedAt TEXT NOT NULL,
              FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
            );
            CREATE TABLE IF NOT EXISTS Attachments (
              Id TEXT PRIMARY KEY,
              CustomerId TEXT NOT NULL,
              FileName TEXT NOT NULL,
              ContentType TEXT NOT NULL,
              SizeBytes INTEGER NOT NULL,
              StoragePath TEXT NOT NULL,
              CreatedAt TEXT NOT NULL,
              FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
            );
            """;
        command.ExecuteNonQuery();
    }

    public void SeedIfEmpty()
    {
        try
        {
            using var connection = Open();
            using (var countCmd = connection.CreateCommand())
            {
                countCmd.CommandText = "SELECT COUNT(1) FROM Customers";
                var count = Convert.ToInt64(countCmd.ExecuteScalar());
                if (count > 0)
                {
                    return;
                }
            }

            var acme = Customer.Register("Acme Industries", "CUST-1001", "Acme Corp", "Active");
            acme.AddContact("email", "ops@acme.example", true);
            acme.AddContact("phone", "+1-555-0100", false);
            acme.AddNote("First call: asked about invoice #4421.", "Demo Agent");
            SaveNew(acme);

            var beta = Customer.Register("Beta LLC", "CUST-1002", "Beta", "Active");
            beta.AddContact("whatsapp", "+1-555-0199", true);
            beta.AddContact("address", "12 Harbor St", false);
            SaveNew(beta);
        }
        catch
        {
            // Seed must never take down startup.
        }
    }

    public Guid? FindIdByUniqueIdentifier(string uniqueIdentifier)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id FROM Customers WHERE UniqueIdentifier = $u LIMIT 1";
        command.Parameters.AddWithValue("$u", uniqueIdentifier.Trim());
        var result = command.ExecuteScalar() as string;
        return result is null ? null : Guid.Parse(result);
    }

    public void InsertCustomer(Customer customer)
    {
        using var connection = Open();
        using var tx = connection.BeginTransaction();
        using (var command = connection.CreateCommand())
        {
            command.Transaction = tx;
            command.CommandText =
                """
                INSERT INTO Customers (Id, DisplayName, UniqueIdentifier, Organization, Status, CreatedAt, UpdatedAt)
                VALUES ($id, $name, $uid, $org, $status, $created, $updated)
                """;
            command.Parameters.AddWithValue("$id", customer.Id.ToString());
            command.Parameters.AddWithValue("$name", customer.DisplayName);
            command.Parameters.AddWithValue("$uid", customer.UniqueIdentifier);
            command.Parameters.AddWithValue("$org", (object?)customer.Organization ?? DBNull.Value);
            command.Parameters.AddWithValue("$status", customer.Status);
            command.Parameters.AddWithValue("$created", customer.CreatedAt.ToString("O"));
            command.Parameters.AddWithValue("$updated", customer.UpdatedAt.ToString("O"));
            command.ExecuteNonQuery();
        }

        foreach (var contact in customer.Contacts)
        {
            InsertContact(connection, tx, contact);
        }

        foreach (var note in customer.Notes)
        {
            InsertNote(connection, tx, note);
        }

        tx.Commit();
    }

    public void UpdateCustomerProfile(Customer customer)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE Customers
            SET DisplayName = $name, UniqueIdentifier = $uid, Organization = $org, Status = $status, UpdatedAt = $updated
            WHERE Id = $id
            """;
        command.Parameters.AddWithValue("$id", customer.Id.ToString());
        command.Parameters.AddWithValue("$name", customer.DisplayName);
        command.Parameters.AddWithValue("$uid", customer.UniqueIdentifier);
        command.Parameters.AddWithValue("$org", (object?)customer.Organization ?? DBNull.Value);
        command.Parameters.AddWithValue("$status", customer.Status);
        command.Parameters.AddWithValue("$updated", customer.UpdatedAt.ToString("O"));
        command.ExecuteNonQuery();
    }

    public void InsertContact(Contact contact)
    {
        using var connection = Open();
        using var tx = connection.BeginTransaction();
        if (contact.IsPrimary)
        {
            using var clear = connection.CreateCommand();
            clear.Transaction = tx;
            clear.CommandText =
                """
                UPDATE Contacts SET IsPrimary = 0
                WHERE CustomerId = $cid AND Type = $type AND IsActive = 1
                """;
            clear.Parameters.AddWithValue("$cid", contact.CustomerId.ToString());
            clear.Parameters.AddWithValue("$type", contact.Type);
            clear.ExecuteNonQuery();
        }

        InsertContact(connection, tx, contact);
        TouchCustomer(connection, tx, contact.CustomerId);
        tx.Commit();
    }

    public void DeactivateContact(Guid customerId, Guid contactId)
    {
        using var connection = Open();
        using var tx = connection.BeginTransaction();
        using (var command = connection.CreateCommand())
        {
            command.Transaction = tx;
            command.CommandText =
                """
                UPDATE Contacts SET IsActive = 0, IsPrimary = 0
                WHERE Id = $id AND CustomerId = $cid
                """;
            command.Parameters.AddWithValue("$id", contactId.ToString());
            command.Parameters.AddWithValue("$cid", customerId.ToString());
            if (command.ExecuteNonQuery() == 0)
            {
                throw new InvalidOperationException("Contact not found.");
            }
        }

        TouchCustomer(connection, tx, customerId);
        tx.Commit();
    }

    public void InsertNote(Note note)
    {
        using var connection = Open();
        using var tx = connection.BeginTransaction();
        InsertNote(connection, tx, note);
        TouchCustomer(connection, tx, note.CustomerId);
        tx.Commit();
    }

    public void InsertAttachment(Attachment attachment)
    {
        using var connection = Open();
        using var tx = connection.BeginTransaction();
        using (var command = connection.CreateCommand())
        {
            command.Transaction = tx;
            command.CommandText =
                """
                INSERT INTO Attachments (Id, CustomerId, FileName, ContentType, SizeBytes, StoragePath, CreatedAt)
                VALUES ($id, $cid, $name, $ct, $size, $path, $created)
                """;
            command.Parameters.AddWithValue("$id", attachment.Id.ToString());
            command.Parameters.AddWithValue("$cid", attachment.CustomerId.ToString());
            command.Parameters.AddWithValue("$name", attachment.FileName);
            command.Parameters.AddWithValue("$ct", attachment.ContentType);
            command.Parameters.AddWithValue("$size", attachment.SizeBytes);
            command.Parameters.AddWithValue("$path", attachment.StoragePath);
            command.Parameters.AddWithValue("$created", attachment.CreatedAt.ToString("O"));
            command.ExecuteNonQuery();
        }

        TouchCustomer(connection, tx, attachment.CustomerId);
        tx.Commit();
    }

    public IReadOnlyList<(Guid Id, string DisplayName, string? Organization, string Status, string UniqueIdentifier)> Search(string? q)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        if (string.IsNullOrWhiteSpace(q))
        {
            command.CommandText =
                """
                SELECT Id, DisplayName, Organization, Status, UniqueIdentifier
                FROM Customers
                ORDER BY DisplayName COLLATE NOCASE
                LIMIT 100
                """;
        }
        else
        {
            command.CommandText =
                """
                SELECT Id, DisplayName, Organization, Status, UniqueIdentifier
                FROM Customers
                WHERE DisplayName LIKE $q OR UniqueIdentifier LIKE $q OR IFNULL(Organization, '') LIKE $q
                ORDER BY DisplayName COLLATE NOCASE
                LIMIT 100
                """;
            command.Parameters.AddWithValue("$q", $"%{q.Trim()}%");
        }

        var rows = new List<(Guid, string, string?, string, string)>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add((
                Guid.Parse(reader.GetString(0)),
                reader.GetString(1),
                reader.IsDBNull(2) ? null : reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4)));
        }

        return rows;
    }

    public Customer? GetCustomer(Guid id)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, DisplayName, UniqueIdentifier, Organization, Status, CreatedAt, UpdatedAt
            FROM Customers WHERE Id = $id
            """;
        command.Parameters.AddWithValue("$id", id.ToString());
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var customerId = Guid.Parse(reader.GetString(0));
        var displayName = reader.GetString(1);
        var uniqueId = reader.GetString(2);
        var organization = reader.IsDBNull(3) ? null : reader.GetString(3);
        var status = reader.GetString(4);
        var createdAt = DateTimeOffset.Parse(reader.GetString(5));
        var updatedAt = DateTimeOffset.Parse(reader.GetString(6));
        reader.Close();

        return Customer.Rehydrate(
            customerId,
            displayName,
            uniqueId,
            organization,
            status,
            createdAt,
            updatedAt,
            LoadContacts(connection, customerId),
            LoadNotes(connection, customerId),
            LoadAttachments(connection, customerId));
    }

    public Attachment? GetAttachment(Guid customerId, Guid attachmentId)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, CustomerId, FileName, ContentType, SizeBytes, StoragePath, CreatedAt
            FROM Attachments WHERE Id = $id AND CustomerId = $cid
            """;
        command.Parameters.AddWithValue("$id", attachmentId.ToString());
        command.Parameters.AddWithValue("$cid", customerId.ToString());
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return Attachment.Rehydrate(
            Guid.Parse(reader.GetString(0)),
            Guid.Parse(reader.GetString(1)),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetInt64(4),
            reader.GetString(5),
            DateTimeOffset.Parse(reader.GetString(6)));
    }

    private void SaveNew(Customer customer) => InsertCustomer(customer);

    private static void InsertContact(SqliteConnection connection, SqliteTransaction tx, Contact contact)
    {
        using var command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText =
            """
            INSERT INTO Contacts (Id, CustomerId, Type, Value, IsPrimary, IsActive, CreatedAt)
            VALUES ($id, $cid, $type, $value, $primary, $active, $created)
            """;
        command.Parameters.AddWithValue("$id", contact.Id.ToString());
        command.Parameters.AddWithValue("$cid", contact.CustomerId.ToString());
        command.Parameters.AddWithValue("$type", contact.Type);
        command.Parameters.AddWithValue("$value", contact.Value);
        command.Parameters.AddWithValue("$primary", contact.IsPrimary ? 1 : 0);
        command.Parameters.AddWithValue("$active", contact.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("$created", contact.CreatedAt.ToString("O"));
        command.ExecuteNonQuery();
    }

    private static void InsertNote(SqliteConnection connection, SqliteTransaction tx, Note note)
    {
        using var command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText =
            """
            INSERT INTO Notes (Id, CustomerId, Body, AuthorName, CreatedAt)
            VALUES ($id, $cid, $body, $author, $created)
            """;
        command.Parameters.AddWithValue("$id", note.Id.ToString());
        command.Parameters.AddWithValue("$cid", note.CustomerId.ToString());
        command.Parameters.AddWithValue("$body", note.Body);
        command.Parameters.AddWithValue("$author", note.AuthorName);
        command.Parameters.AddWithValue("$created", note.CreatedAt.ToString("O"));
        command.ExecuteNonQuery();
    }

    private static void TouchCustomer(SqliteConnection connection, SqliteTransaction tx, Guid customerId)
    {
        using var command = connection.CreateCommand();
        command.Transaction = tx;
        command.CommandText = "UPDATE Customers SET UpdatedAt = $u WHERE Id = $id";
        command.Parameters.AddWithValue("$u", DateTimeOffset.UtcNow.ToString("O"));
        command.Parameters.AddWithValue("$id", customerId.ToString());
        command.ExecuteNonQuery();
    }

    private static List<Contact> LoadContacts(SqliteConnection connection, Guid customerId)
    {
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, CustomerId, Type, Value, IsPrimary, IsActive, CreatedAt
            FROM Contacts WHERE CustomerId = $cid ORDER BY CreatedAt
            """;
        command.Parameters.AddWithValue("$cid", customerId.ToString());
        var list = new List<Contact>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(Contact.Rehydrate(
                Guid.Parse(reader.GetString(0)),
                Guid.Parse(reader.GetString(1)),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetInt64(4) == 1,
                reader.GetInt64(5) == 1,
                DateTimeOffset.Parse(reader.GetString(6))));
        }

        return list;
    }

    private static List<Note> LoadNotes(SqliteConnection connection, Guid customerId)
    {
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, CustomerId, Body, AuthorName, CreatedAt
            FROM Notes WHERE CustomerId = $cid ORDER BY CreatedAt
            """;
        command.Parameters.AddWithValue("$cid", customerId.ToString());
        var list = new List<Note>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(Note.Rehydrate(
                Guid.Parse(reader.GetString(0)),
                Guid.Parse(reader.GetString(1)),
                reader.GetString(2),
                reader.GetString(3),
                DateTimeOffset.Parse(reader.GetString(4))));
        }

        return list;
    }

    private static List<Attachment> LoadAttachments(SqliteConnection connection, Guid customerId)
    {
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, CustomerId, FileName, ContentType, SizeBytes, StoragePath, CreatedAt
            FROM Attachments WHERE CustomerId = $cid ORDER BY CreatedAt
            """;
        command.Parameters.AddWithValue("$cid", customerId.ToString());
        var list = new List<Attachment>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(Attachment.Rehydrate(
                Guid.Parse(reader.GetString(0)),
                Guid.Parse(reader.GetString(1)),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetInt64(4),
                reader.GetString(5),
                DateTimeOffset.Parse(reader.GetString(6))));
        }

        return list;
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
