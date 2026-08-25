using Crm.Customers.Api.Domain;
using Crm.Customers.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Crm.Customers.Api.Infrastructure;

/// <summary>SDD CRM-001 / CRM-037 — EF Core facade (same sync API as prior ADO store).</summary>
public sealed class CustomersDb
{
    private readonly IDbContextFactory<CustomersDbContext> _factory;
    private readonly string _attachmentsRoot;

    public CustomersDb(
        IDbContextFactory<CustomersDbContext> factory,
        IWebHostEnvironment env,
        IConfiguration config)
    {
        _factory = factory;
        var dataRoot = Path.GetFullPath(config["Customers:DataPath"] ?? Path.Combine(env.ContentRootPath, "data"));
        Directory.CreateDirectory(dataRoot);
        _attachmentsRoot = Path.Combine(dataRoot, "attachments");
        Directory.CreateDirectory(_attachmentsRoot);
    }

    public string AttachmentsRoot => _attachmentsRoot;

    public void EnsureSchema()
    {
        using var db = _factory.CreateDbContext();
        db.Database.EnsureCreated();
    }

    public void SeedIfEmpty()
    {
        try
        {
            using (var db = _factory.CreateDbContext())
            {
                if (db.Customers.Any())
                {
                    return;
                }
            }

            var acme = Customer.Register(
                "Acme Industries",
                "CUST-1001",
                "Acme Corp",
                "Active",
                Guid.Parse("0a50ed5a-1796-4555-bc08-5b28cf59a539"));
            acme.AddContact("email", "ops@acme.example", true);
            acme.AddContact("phone", "+1-555-0100", false);
            acme.AddNote("First call: asked about invoice #4421.", "Demo Agent");
            InsertCustomer(acme);

            var beta = Customer.Register(
                "Beta LLC",
                "CUST-1002",
                "Beta",
                "Active",
                Guid.Parse("72f13bfd-ef8f-45b0-b7e0-57b3d353a70b"));
            beta.AddContact("whatsapp", "+1-555-0199", true);
            beta.AddContact("address", "12 Harbor St", false);
            InsertCustomer(beta);
        }
        catch
        {
            // Seed must never take down startup.
        }
    }

    public Guid? FindIdByUniqueIdentifier(string uniqueIdentifier)
    {
        using var db = _factory.CreateDbContext();
        var key = uniqueIdentifier.Trim();
        var match = db.Customers.AsNoTracking()
            .AsEnumerable()
            .FirstOrDefault(c => c.UniqueIdentifier.Equals(key, StringComparison.OrdinalIgnoreCase));
        return match?.Id;
    }

    public void InsertCustomer(Customer customer)
    {
        using var db = _factory.CreateDbContext();
        db.Customers.Add(ToRow(customer));
        foreach (var contact in customer.Contacts)
        {
            db.Contacts.Add(ToRow(contact));
        }

        foreach (var note in customer.Notes)
        {
            db.Notes.Add(ToRow(note));
        }

        db.SaveChanges();
    }

    public void UpdateCustomerProfile(Customer customer)
    {
        using var db = _factory.CreateDbContext();
        var row = db.Customers.FirstOrDefault(c => c.Id == customer.Id)
                  ?? throw new InvalidOperationException("Customer not found.");
        row.DisplayName = customer.DisplayName;
        row.UniqueIdentifier = customer.UniqueIdentifier;
        row.Organization = customer.Organization;
        row.Status = customer.Status;
        row.UpdatedAt = customer.UpdatedAt;
        db.SaveChanges();
    }

    public void InsertContact(Contact contact)
    {
        using var db = _factory.CreateDbContext();
        if (contact.IsPrimary)
        {
            var others = db.Contacts
                .Where(c => c.CustomerId == contact.CustomerId
                            && c.Type == contact.Type
                            && c.IsActive)
                .ToList();
            foreach (var other in others)
            {
                other.IsPrimary = false;
            }
        }

        db.Contacts.Add(ToRow(contact));
        Touch(db, contact.CustomerId);
        db.SaveChanges();
    }

    public void DeactivateContact(Guid customerId, Guid contactId)
    {
        using var db = _factory.CreateDbContext();
        var row = db.Contacts.FirstOrDefault(c => c.Id == contactId && c.CustomerId == customerId)
                  ?? throw new InvalidOperationException("Contact not found.");
        row.IsActive = false;
        row.IsPrimary = false;
        Touch(db, customerId);
        db.SaveChanges();
    }

    public void InsertNote(Note note)
    {
        using var db = _factory.CreateDbContext();
        db.Notes.Add(ToRow(note));
        Touch(db, note.CustomerId);
        db.SaveChanges();
    }

    public void InsertAttachment(Attachment attachment)
    {
        using var db = _factory.CreateDbContext();
        db.Attachments.Add(ToRow(attachment));
        Touch(db, attachment.CustomerId);
        db.SaveChanges();
    }

    public IReadOnlyList<(Guid Id, string DisplayName, string? Organization, string Status, string UniqueIdentifier)> Search(string? q)
    {
        using var db = _factory.CreateDbContext();
        IQueryable<CustomerRow> query = db.Customers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            query = query.Where(c =>
                c.DisplayName.ToLower().Contains(term)
                || c.UniqueIdentifier.ToLower().Contains(term)
                || (c.Organization != null && c.Organization.ToLower().Contains(term)));
        }

        return query
            .OrderBy(c => c.DisplayName)
            .Take(100)
            .ToList()
            .Select(c => (c.Id, c.DisplayName, c.Organization, c.Status, c.UniqueIdentifier))
            .ToList();
    }

    public Customer? GetCustomer(Guid id)
    {
        using var db = _factory.CreateDbContext();
        var row = db.Customers.AsNoTracking().FirstOrDefault(c => c.Id == id);
        if (row is null)
        {
            return null;
        }

        var contacts = db.Contacts.AsNoTracking()
            .Where(c => c.CustomerId == id)
            .ToList()
            .OrderBy(c => c.CreatedAt)
            .Select(c => Contact.Rehydrate(c.Id, c.CustomerId, c.Type, c.Value, c.IsPrimary, c.IsActive, c.CreatedAt));
        var notes = db.Notes.AsNoTracking()
            .Where(n => n.CustomerId == id)
            .ToList()
            .OrderBy(n => n.CreatedAt)
            .Select(n => Note.Rehydrate(n.Id, n.CustomerId, n.Body, n.AuthorName, n.CreatedAt));
        var attachments = db.Attachments.AsNoTracking()
            .Where(a => a.CustomerId == id)
            .ToList()
            .OrderBy(a => a.CreatedAt)
            .Select(a => Attachment.Rehydrate(
                a.Id, a.CustomerId, a.FileName, a.ContentType, a.SizeBytes, a.StoragePath, a.CreatedAt));

        return Customer.Rehydrate(
            row.Id,
            row.DisplayName,
            row.UniqueIdentifier,
            row.Organization,
            row.Status,
            row.CreatedAt,
            row.UpdatedAt,
            contacts,
            notes,
            attachments);
    }

    public Attachment? GetAttachment(Guid customerId, Guid attachmentId)
    {
        using var db = _factory.CreateDbContext();
        var a = db.Attachments.AsNoTracking()
            .FirstOrDefault(x => x.Id == attachmentId && x.CustomerId == customerId);
        return a is null
            ? null
            : Attachment.Rehydrate(
                a.Id, a.CustomerId, a.FileName, a.ContentType, a.SizeBytes, a.StoragePath, a.CreatedAt);
    }

    private static void Touch(CustomersDbContext db, Guid customerId)
    {
        var row = db.Customers.FirstOrDefault(c => c.Id == customerId);
        if (row is not null)
        {
            row.UpdatedAt = DateTimeOffset.UtcNow;
        }
    }

    private static CustomerRow ToRow(Customer c) => new()
    {
        Id = c.Id,
        DisplayName = c.DisplayName,
        UniqueIdentifier = c.UniqueIdentifier,
        Organization = c.Organization,
        Status = c.Status,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };

    private static ContactRow ToRow(Contact c) => new()
    {
        Id = c.Id,
        CustomerId = c.CustomerId,
        Type = c.Type,
        Value = c.Value,
        IsPrimary = c.IsPrimary,
        IsActive = c.IsActive,
        CreatedAt = c.CreatedAt
    };

    private static NoteRow ToRow(Note n) => new()
    {
        Id = n.Id,
        CustomerId = n.CustomerId,
        Body = n.Body,
        AuthorName = n.AuthorName,
        CreatedAt = n.CreatedAt
    };

    private static AttachmentRow ToRow(Attachment a) => new()
    {
        Id = a.Id,
        CustomerId = a.CustomerId,
        FileName = a.FileName,
        ContentType = a.ContentType,
        SizeBytes = a.SizeBytes,
        StoragePath = a.StoragePath,
        CreatedAt = a.CreatedAt
    };
}
