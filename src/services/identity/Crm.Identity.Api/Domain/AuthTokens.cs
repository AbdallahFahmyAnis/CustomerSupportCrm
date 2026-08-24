namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-035 — refresh token row.</summary>
public sealed class StoredRefreshToken
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string TokenHash { get; init; } = "";
    public DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? RevokedAt { get; init; }
    public Guid? ReplacedByTokenId { get; init; }

    public bool IsActive(DateTimeOffset utcNow)
        => RevokedAt is null && ExpiresAt > utcNow;
}

/// <summary>SDD CRM-035 — revoked access JWT jti until expiry.</summary>
public sealed class RevokedAccessToken
{
    public string Jti { get; init; } = "";
    public Guid UserId { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public DateTimeOffset RevokedAt { get; init; }
}
