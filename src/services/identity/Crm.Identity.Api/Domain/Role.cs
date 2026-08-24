namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-035 — role aggregate with permission set.</summary>
public sealed class Role
{
    public string Name { get; private set; } = "";
    public string Description { get; private set; } = "";
    public IReadOnlyList<string> Permissions { get; private set; } = [];

    public static Role Define(string name, string description, IEnumerable<string> permissions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return new Role
        {
            Name = name.Trim(),
            Description = description.Trim(),
            Permissions = permissions.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(p => p).ToList()
        };
    }

    public static Role Rehydrate(string name, string description, IEnumerable<string> permissions)
        => Define(name, description, permissions);
}
