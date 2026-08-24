namespace Crm.Customers.Api.Domain;

/// <summary>SDD CRM-001 / specs/002-customer-profiles — Customer aggregate root.</summary>
public sealed class Customer
{
    private readonly List<Contact> _contacts = [];
    private readonly List<Note> _notes = [];
    private readonly List<Attachment> _attachments = [];

    public Guid Id { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string UniqueIdentifier { get; private set; } = string.Empty;
    public string? Organization { get; private set; }
    public string Status { get; private set; } = "Active";
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public IReadOnlyList<Contact> Contacts => _contacts;
    public IReadOnlyList<Note> Notes => _notes;
    public IReadOnlyList<Attachment> Attachments => _attachments;

    private Customer()
    {
    }

    public static Customer Register(
        string displayName,
        string uniqueIdentifier,
        string? organization = null,
        string? status = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(uniqueIdentifier);

        var now = DateTimeOffset.UtcNow;
        return new Customer
        {
            Id = Guid.NewGuid(),
            DisplayName = displayName.Trim(),
            UniqueIdentifier = uniqueIdentifier.Trim(),
            Organization = string.IsNullOrWhiteSpace(organization) ? null : organization.Trim(),
            Status = string.IsNullOrWhiteSpace(status) ? "Active" : status.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public static Customer Rehydrate(
        Guid id,
        string displayName,
        string uniqueIdentifier,
        string? organization,
        string status,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt,
        IEnumerable<Contact>? contacts = null,
        IEnumerable<Note>? notes = null,
        IEnumerable<Attachment>? attachments = null)
    {
        var customer = new Customer
        {
            Id = id,
            DisplayName = displayName,
            UniqueIdentifier = uniqueIdentifier,
            Organization = organization,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
        if (contacts is not null)
        {
            customer._contacts.AddRange(contacts);
        }

        if (notes is not null)
        {
            customer._notes.AddRange(notes);
        }

        if (attachments is not null)
        {
            customer._attachments.AddRange(attachments);
        }

        return customer;
    }

    public void UpdateProfile(string displayName, string uniqueIdentifier, string? organization, string? status)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(uniqueIdentifier);
        DisplayName = displayName.Trim();
        UniqueIdentifier = uniqueIdentifier.Trim();
        Organization = string.IsNullOrWhiteSpace(organization) ? null : organization.Trim();
        Status = string.IsNullOrWhiteSpace(status) ? Status : status.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Contact AddContact(string type, string value, bool isPrimary)
    {
        var contact = Contact.Create(Id, type, value, isPrimary);
        if (isPrimary)
        {
            foreach (var existing in _contacts.Where(c => c.IsActive && c.Type.Equals(type, StringComparison.OrdinalIgnoreCase)))
            {
                existing.ClearPrimary();
            }
        }

        _contacts.Add(contact);
        UpdatedAt = DateTimeOffset.UtcNow;
        return contact;
    }

    public void DeactivateContact(Guid contactId)
    {
        var contact = _contacts.FirstOrDefault(c => c.Id == contactId)
            ?? throw new InvalidOperationException("Contact not found.");
        contact.Deactivate();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Note AddNote(string body, string authorName)
    {
        var note = Note.Create(Id, body, authorName);
        _notes.Add(note);
        UpdatedAt = DateTimeOffset.UtcNow;
        return note;
    }

    public Attachment AddAttachment(string fileName, string contentType, long sizeBytes, string storagePath)
    {
        var attachment = Attachment.Create(Id, fileName, contentType, sizeBytes, storagePath);
        _attachments.Add(attachment);
        UpdatedAt = DateTimeOffset.UtcNow;
        return attachment;
    }
}
