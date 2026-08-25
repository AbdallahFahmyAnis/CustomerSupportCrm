namespace Crm.Knowledge.Api.Domain;

/// <summary>SDD CRM-021 — knowledge article aggregate.</summary>
public sealed class Article
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = "";
    public string Body { get; private set; } = "";
    public string Kind { get; private set; } = "";
    public string Status { get; private set; } = "";
    public string CreatedBy { get; private set; } = "";
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Article()
    {
    }

    public static Article Create(string title, string body, string kind, string status, string createdBy)
    {
        Validate(title, body);
        var now = DateTimeOffset.UtcNow;
        return new Article
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Body = body.Trim(),
            Kind = KnowledgeCatalog.NormalizeKind(kind),
            Status = KnowledgeCatalog.NormalizeStatus(status),
            CreatedBy = string.IsNullOrWhiteSpace(createdBy) ? "System" : createdBy.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public static Article Rehydrate(
        Guid id,
        string title,
        string body,
        string kind,
        string status,
        string createdBy,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt) => new()
    {
        Id = id,
        Title = title,
        Body = body,
        Kind = kind,
        Status = status,
        CreatedBy = createdBy,
        CreatedAt = createdAt,
        UpdatedAt = updatedAt
    };

    public void Update(string title, string body, string kind, string status)
    {
        Validate(title, body);
        Title = title.Trim();
        Body = body.Trim();
        Kind = KnowledgeCatalog.NormalizeKind(kind);
        Status = KnowledgeCatalog.NormalizeStatus(status);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void Validate(string title, string body)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.");
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentException("Body is required.");
        }
    }
}
