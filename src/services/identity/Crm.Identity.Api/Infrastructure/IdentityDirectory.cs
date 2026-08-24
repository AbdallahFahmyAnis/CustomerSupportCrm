using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Crm.Identity.Api.Infrastructure;

/// <summary>
/// SDD CRM-035 / CRM-037 — user/role/token directory over ASP.NET Identity + EF Core.
/// OpenIddict is registered alongside for OIDC password/refresh (/connect/token).
/// </summary>
public sealed class IdentityDirectory(
    UserManager<ApplicationUser> users,
    RoleManager<IdentityRole<Guid>> roles,
    IdentityAppDbContext db,
    TokenService tokens)
{
    public async Task EnsureSchemaAsync(CancellationToken cancellationToken = default)
    {
        await db.Database.EnsureCreatedAsync(cancellationToken);
    }

    public async Task<UserAccount?> FindByEmailAsync(string email, CancellationToken ct = default)
    {
        var entity = await users.FindByEmailAsync(email.Trim().ToLowerInvariant());
        return entity is null ? null : await ToAccountAsync(entity);
    }

    public async Task<UserAccount?> GetUserAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await users.FindByIdAsync(id.ToString());
        return entity is null ? null : await ToAccountAsync(entity);
    }

    public async Task<IReadOnlyList<UserAccount>> SearchUsersAsync(string? q, CancellationToken ct = default)
    {
        var query = users.Users.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLowerInvariant();
            query = query.Where(u =>
                (u.Email != null && u.Email.Contains(term)) ||
                u.DisplayName.ToLower().Contains(term));
        }

        var list = await query.OrderBy(u => u.DisplayName).Take(100).ToListAsync(ct);
        var accounts = new List<UserAccount>();
        foreach (var user in list)
        {
            accounts.Add(await ToAccountAsync(user));
        }

        return accounts;
    }

    public async Task<(UserAccount? User, string? Error)> CreateUserAsync(
        string email,
        string displayName,
        string password,
        string role,
        CancellationToken ct = default)
    {
        try
        {
            UserAccount.EnsureRolePublic(role);
        }
        catch (Exception ex)
        {
            return (null, ex.Message);
        }

        email = email.Trim().ToLowerInvariant();
        if (await users.FindByEmailAsync(email) is not null)
        {
            return (null, "A user with that email already exists.");
        }

        var entity = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            DisplayName = displayName.Trim(),
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var create = await users.CreateAsync(entity, password);
        if (!create.Succeeded)
        {
            return (null, string.Join("; ", create.Errors.Select(e => e.Description)));
        }

        await users.AddToRoleAsync(entity, role.Trim());
        return (await ToAccountAsync(entity), null);
    }

    public async Task UpdateAsync(UserAccount account, CancellationToken ct = default)
    {
        var entity = await users.FindByIdAsync(account.Id.ToString())
                     ?? throw new InvalidOperationException("User not found.");

        entity.DisplayName = account.DisplayName;
        entity.IsActive = account.IsActive;
        if (!account.IsActive)
        {
            entity.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
        }
        else if (account.LockoutUntil is null)
        {
            entity.LockoutEnd = null;
            await users.ResetAccessFailedCountAsync(entity);
        }
        else
        {
            entity.LockoutEnd = account.LockoutUntil;
        }

        await users.UpdateAsync(entity);

        var currentRoles = await users.GetRolesAsync(entity);
        if (!currentRoles.Contains(account.Role, StringComparer.OrdinalIgnoreCase))
        {
            if (currentRoles.Count > 0)
            {
                await users.RemoveFromRolesAsync(entity, currentRoles);
            }

            await users.AddToRoleAsync(entity, account.Role);
        }
    }

    public async Task RegisterFailedLoginAsync(UserAccount account, DateTimeOffset now, CancellationToken ct = default)
    {
        var entity = await users.FindByIdAsync(account.Id.ToString());
        if (entity is null)
        {
            return;
        }

        await users.AccessFailedAsync(entity);
    }

    public async Task RegisterSuccessfulLoginAsync(UserAccount account, CancellationToken ct = default)
    {
        var entity = await users.FindByIdAsync(account.Id.ToString());
        if (entity is null)
        {
            return;
        }

        await users.ResetAccessFailedCountAsync(entity);
        await users.SetLockoutEndDateAsync(entity, null);
    }

    public async Task<bool> CheckPasswordAsync(UserAccount account, string password)
    {
        var entity = await users.FindByIdAsync(account.Id.ToString());
        return entity is not null && await users.CheckPasswordAsync(entity, password);
    }

    public async Task<bool> IsLockedOutAsync(Guid userId, CancellationToken ct = default)
    {
        var entity = await users.FindByIdAsync(userId.ToString());
        return entity is not null && await users.IsLockedOutAsync(entity);
    }

    public async Task InsertRefreshTokenAsync(StoredRefreshToken token, CancellationToken ct = default)
    {
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync(ct);
    }

    public async Task<StoredRefreshToken?> FindRefreshTokenByHashAsync(string hash, CancellationToken ct = default)
        => await db.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

    public async Task RevokeRefreshTokenAsync(Guid id, DateTimeOffset revokedAt, Guid? replacedBy = null, CancellationToken ct = default)
    {
        var entity = await db.RefreshTokens.FirstOrDefaultAsync(t => t.Id == id && t.RevokedAt == null, ct);
        if (entity is null)
        {
            return;
        }

        // EF entity is Domain class with init-only — attach update via ExecuteUpdate
        await db.RefreshTokens
            .Where(t => t.Id == id && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.RevokedAt, revokedAt)
                .SetProperty(t => t.ReplacedByTokenId, replacedBy), ct);
    }

    public async Task RevokeAllRefreshTokensForUserAsync(Guid userId, DateTimeOffset revokedAt, CancellationToken ct = default)
    {
        await db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, revokedAt), ct);
    }

    public async Task RevokeAccessJtiAsync(string jti, Guid userId, DateTimeOffset expiresAt, DateTimeOffset revokedAt, CancellationToken ct = default)
    {
        var existing = await db.RevokedAccessTokens.FindAsync([jti], ct);
        if (existing is null)
        {
            db.RevokedAccessTokens.Add(new RevokedAccessToken
            {
                Jti = jti,
                UserId = userId,
                ExpiresAt = expiresAt,
                RevokedAt = revokedAt
            });
        }
        else
        {
            await db.RevokedAccessTokens
                .Where(t => t.Jti == jti)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.UserId, userId)
                    .SetProperty(t => t.ExpiresAt, expiresAt)
                    .SetProperty(t => t.RevokedAt, revokedAt), ct);
            return;
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Role>> ListRolesAsync(CancellationToken ct = default)
    {
        var all = await roles.Roles.AsNoTracking().OrderBy(r => r.Name).ToListAsync(ct);
        var result = new List<Role>();
        foreach (var role in all)
        {
            if (role.Name is null)
            {
                continue;
            }

            result.Add(await ToRoleAsync(role));
        }

        return result;
    }

    public async Task<TokenResponseDto> IssuePairAsync(UserAccount user, DateTimeOffset now, CancellationToken ct = default)
    {
        var (access, _, accessExp) = tokens.CreateAccessToken(
            new TokenService.UserClaims(user.Id, user.Email, user.DisplayName, user.Role));
        var refreshValue = TokenService.CreateRefreshTokenValue();
        var refresh = new StoredRefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = TokenService.HashToken(refreshValue),
            ExpiresAt = now.Add(tokens.RefreshLifetime),
            CreatedAt = now
        };
        db.RefreshTokens.Add(refresh);
        await db.SaveChangesAsync(ct);

        return new TokenResponseDto(
            access,
            refreshValue,
            accessExp,
            refresh.ExpiresAt,
            new DevUserDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role));
    }

    private async Task<UserAccount> ToAccountAsync(ApplicationUser entity)
    {
        var roleList = await users.GetRolesAsync(entity);
        var role = roleList.FirstOrDefault() ?? RoleNames.Agent;
        DateTimeOffset? lockoutUntil = null;
        if (entity.LockoutEnd.HasValue && entity.LockoutEnd > DateTimeOffset.UtcNow)
        {
            lockoutUntil = entity.LockoutEnd;
        }

        return UserAccount.Rehydrate(
            entity.Id,
            entity.Email ?? "",
            entity.DisplayName,
            entity.PasswordHash ?? "",
            role,
            entity.IsActive,
            entity.AccessFailedCount,
            lockoutUntil,
            entity.CreatedAt);
    }

    private async Task<Role> ToRoleAsync(IdentityRole<Guid> role)
    {
        var claims = await roles.GetClaimsAsync(role);
        var description = claims.FirstOrDefault(c => c.Type == "role_description")?.Value
                          ?? role.Name
                          ?? "";
        var permissions = claims
            .Where(c => c.Type == IdentityDataSeeder.PermissionClaimType)
            .Select(c => c.Value)
            .ToList();
        return Role.Rehydrate(role.Name!, description, permissions);
    }
}
