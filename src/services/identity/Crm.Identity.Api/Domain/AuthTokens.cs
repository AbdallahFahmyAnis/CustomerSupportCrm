namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-035 — refresh token row (EF).</summary>
public sealed class StoredRefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = "";
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public Guid? ReplacedByTokenId { get; set; }

    public bool IsActive(DateTimeOffset utcNow)
        => RevokedAt is null && ExpiresAt > utcNow;
}

/// <summary>SDD CRM-035 — revoked access JWT jti until expiry (EF).</summary>
public sealed class RevokedAccessToken
{
    public string Jti { get; set; } = "";
    public Guid UserId { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset RevokedAt { get; set; }
}
