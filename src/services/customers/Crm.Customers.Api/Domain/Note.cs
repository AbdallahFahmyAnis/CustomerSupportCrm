namespace Crm.Customers.Api.Domain;

/// <summary>SDD CRM-003 / specs/002-customer-profiles — internal note on a customer.</summary>
public sealed class Note
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Body { get; private set; } = string.Empty;
    public string AuthorName { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    private Note()
    {
    }

    public static Note Create(Guid customerId, string body, string authorName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(body);
        return new Note
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Body = body.Trim(),
            AuthorName = string.IsNullOrWhiteSpace(authorName) ? "Agent" : authorName.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static Note Rehydrate(Guid id, Guid customerId, string body, string authorName, DateTimeOffset createdAt)
        => new()
        {
            Id = id,
            CustomerId = customerId,
            Body = body,
            AuthorName = authorName,
            CreatedAt = createdAt
        };
}
