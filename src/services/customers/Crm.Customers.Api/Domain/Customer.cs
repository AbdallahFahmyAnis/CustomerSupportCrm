namespace Crm.Customers.Api.Domain;

/// <summary>SDD 001-platform-foundation — Customer aggregate placeholder; persistence arrives in CRM-001.</summary>
public sealed class Customer
{
    public Guid Id { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string UniqueIdentifier { get; private set; } = string.Empty;

    private Customer()
    {
    }

    public static Customer Register(string displayName, string uniqueIdentifier)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(uniqueIdentifier);

        return new Customer
        {
            Id = Guid.NewGuid(),
            DisplayName = displayName.Trim(),
            UniqueIdentifier = uniqueIdentifier.Trim()
        };
    }
}
