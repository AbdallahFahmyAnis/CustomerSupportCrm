using System.Security.Cryptography;
using System.Text;

namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-035 — user aggregate with lockout.</summary>
public sealed class UserAccount
{
    public const int MaxFailedAttempts = 5;
    public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    public Guid Id { get; private set; }
    public string Email { get; private set; } = "";
    public string DisplayName { get; private set; } = "";
    public string PasswordHash { get; private set; } = "";
    public string Role { get; private set; } = RoleNames.Agent;
    public bool IsActive { get; private set; } = true;
    public int FailedLoginCount { get; private set; }
    public DateTimeOffset? LockoutUntil { get; private set; }
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
            FailedLoginCount = 0,
            LockoutUntil = null,
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
        int failedLoginCount,
        DateTimeOffset? lockoutUntil,
        DateTimeOffset createdAt)
        => new()
        {
            Id = id,
            Email = email,
            DisplayName = displayName,
            PasswordHash = passwordHash,
            Role = role,
            IsActive = isActive,
            FailedLoginCount = failedLoginCount,
            LockoutUntil = lockoutUntil,
            CreatedAt = createdAt
        };

    public bool IsLockedOut(DateTimeOffset utcNow)
        => LockoutUntil is not null && LockoutUntil > utcNow;

    public bool VerifyPassword(string password)
        => IsActive && PasswordHash == HashPassword(password);

    public void RegisterFailedLogin(DateTimeOffset utcNow)
    {
        FailedLoginCount++;
        if (FailedLoginCount >= MaxFailedAttempts)
        {
            LockoutUntil = utcNow.Add(LockoutDuration);
            FailedLoginCount = 0;
        }
    }

    public void RegisterSuccessfulLogin()
    {
        FailedLoginCount = 0;
        LockoutUntil = null;
    }

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

    public static void EnsureRolePublic(string role) => EnsureRole(role);

    private static void EnsureRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role) ||
            !RoleNames.All.Contains(role.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Role must be one of: " + string.Join(", ", RoleNames.All));
        }
    }
}
