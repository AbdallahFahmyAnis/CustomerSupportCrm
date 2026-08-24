using System.Security.Cryptography;
using System.Text;

namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-035 — user aggregate.</summary>
public sealed class UserAccount
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = "";
    public string DisplayName { get; private set; } = "";
    public string PasswordHash { get; private set; } = "";
    public string Role { get; private set; } = RoleNames.Agent;
    public bool IsActive { get; private set; } = true;
    public DateTimeOffset CreatedAt { get; private set; }

    public static UserAccount Register(
        string email,
        string displayName,
        string password,
        string role,
        Guid? id = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        EnsureRole(role);

        return new UserAccount
        {
            Id = id ?? Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            DisplayName = displayName.Trim(),
            PasswordHash = HashPassword(password),
            Role = role.Trim(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static UserAccount Rehydrate(
        Guid id,
        string email,
        string displayName,
        string passwordHash,
        string role,
        bool isActive,
        DateTimeOffset createdAt)
        => new()
        {
            Id = id,
            Email = email,
            DisplayName = displayName,
            PasswordHash = passwordHash,
            Role = role,
            IsActive = isActive,
            CreatedAt = createdAt
        };

    public bool VerifyPassword(string password)
        => IsActive && PasswordHash == HashPassword(password);

    public void AssignRole(string role)
    {
        EnsureRole(role);
        Role = role.Trim();
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes("crm:" + password));
        return Convert.ToHexString(bytes);
    }

    private static void EnsureRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role) ||
            !RoleNames.All.Contains(role.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Role must be one of: " + string.Join(", ", RoleNames.All));
        }
    }
}
