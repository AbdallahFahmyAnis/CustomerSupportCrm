namespace Crm.Customers.Api.Domain;

/// <summary>SDD CRM-002 / specs/002-customer-profiles — contact value on a customer.</summary>
public sealed class Contact
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public bool IsPrimary { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }

    private Contact()
    {
    }

    public static Contact Create(Guid customerId, string type, string value, bool isPrimary)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(type);
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        var normalized = NormalizeType(type);
        return new Contact
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Type = normalized,
            Value = value.Trim(),
            IsPrimary = isPrimary,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static Contact Rehydrate(
        Guid id,
        Guid customerId,
        string type,
        string value,
        bool isPrimary,
        bool isActive,
        DateTimeOffset createdAt)
        => new()
        {
            Id = id,
            CustomerId = customerId,
            Type = type,
            Value = value,
            IsPrimary = isPrimary,
            IsActive = isActive,
            CreatedAt = createdAt
        };

    public void ClearPrimary() => IsPrimary = false;

    public void Deactivate()
    {
        IsActive = false;
        IsPrimary = false;
    }

    private static string NormalizeType(string type)
    {
        var t = type.Trim().ToLowerInvariant();
        return t switch
        {
            "email" or "phone" or "whatsapp" or "address" => t,
            _ => throw new ArgumentException("Contact type must be email, phone, whatsapp, or address.")
        };
    }
}
