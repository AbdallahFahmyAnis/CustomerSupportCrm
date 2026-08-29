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
        await EnsureAuditTableAsync(cancellationToken);
        await EnsureSystemSettingsTableAsync(cancellationToken);
        await EnsurePermissionDefinitionsTableAsync(cancellationToken);
        await EnsureDepartmentsTablesAsync(cancellationToken);
        await EnsureUserOrgColumnsAsync(cancellationToken);
    }

    /// <summary>SDD CRM-036 / specs/051 — EnsureCreated will not add tables to an existing DB.</summary>
    private async Task EnsureAuditTableAsync(CancellationToken cancellationToken)
    {
        if (db.Database.IsSqlite())
        {
            await db.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE IF NOT EXISTS "AuditLogs" (
                    "Id" TEXT NOT NULL CONSTRAINT "PK_AuditLogs" PRIMARY KEY,
                    "OccurredAt" TEXT NOT NULL,
                    "Action" TEXT NOT NULL,
                    "ActorUserId" TEXT NULL,
                    "ActorEmail" TEXT NULL,
                    "TargetUserId" TEXT NULL,
                    "TargetEmail" TEXT NULL,
                    "Detail" TEXT NULL,
                    "Success" INTEGER NOT NULL,
                    "Service" TEXT NOT NULL DEFAULT 'Identity'
                );
                """,
                cancellationToken);
            try
            {
                await db.Database.ExecuteSqlRawAsync(
                    """ALTER TABLE "AuditLogs" ADD COLUMN "Service" TEXT NOT NULL DEFAULT 'Identity';""",
                    cancellationToken);
            }
            catch
            {
                // column exists
            }

            return;
        }

        await db.Database.ExecuteSqlRawAsync(
            """
            IF OBJECT_ID(N'[AuditLogs]', N'U') IS NULL
            BEGIN
              CREATE TABLE [AuditLogs] (
                [Id] uniqueidentifier NOT NULL CONSTRAINT [PK_AuditLogs] PRIMARY KEY,
                [OccurredAt] datetimeoffset NOT NULL,
                [Action] nvarchar(100) NOT NULL,
                [ActorUserId] uniqueidentifier NULL,
                [ActorEmail] nvarchar(256) NULL,
                [TargetUserId] uniqueidentifier NULL,
                [TargetEmail] nvarchar(256) NULL,
                [Detail] nvarchar(1000) NULL,
                [Success] bit NOT NULL,
                [Service] nvarchar(64) NOT NULL CONSTRAINT [DF_AuditLogs_Service] DEFAULT N'Identity'
              );
            END
            """,
            cancellationToken);
        await db.Database.ExecuteSqlRawAsync(
            """
            IF COL_LENGTH(N'AuditLogs', N'Service') IS NULL
              ALTER TABLE [AuditLogs] ADD [Service] nvarchar(64) NOT NULL
                CONSTRAINT [DF_AuditLogs_Service] DEFAULT N'Identity';
            """,
            cancellationToken);
    }

    /// <summary>SDD CRM-037 — EnsureCreated will not add tables to an existing DB.</summary>
    private async Task EnsureSystemSettingsTableAsync(CancellationToken cancellationToken)
    {
        if (db.Database.IsSqlite())
        {
            await db.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE IF NOT EXISTS "SystemSettings" (
                    "Id" TEXT NOT NULL CONSTRAINT "PK_SystemSettings" PRIMARY KEY,
                    "OrganizationName" TEXT NOT NULL,
                    "SupportEmail" TEXT NOT NULL,
                    "DefaultCulture" TEXT NOT NULL,
                    "MaxFailedLoginAttempts" INTEGER NOT NULL,
                    "LockoutMinutes" INTEGER NOT NULL,
                    "UpdatedAt" TEXT NOT NULL
                );
                """);
            foreach (var sql in new[]
                     {
                         """ALTER TABLE "SystemSettings" ADD COLUMN "ProductTitle" TEXT NOT NULL DEFAULT 'Customer Support CRM';""",
                         """ALTER TABLE "SystemSettings" ADD COLUMN "PrimaryColor" TEXT NOT NULL DEFAULT '#2563eb';""",
                         """ALTER TABLE "SystemSettings" ADD COLUMN "LogoUrl" TEXT NOT NULL DEFAULT '/brand/azm-squad.png';""",
                         """ALTER TABLE "SystemSettings" ADD COLUMN "ErpWebhookUrl" TEXT NOT NULL DEFAULT '';""",
                         """ALTER TABLE "SystemSettings" ADD COLUMN "ErpWebhookAuthHeader" TEXT NOT NULL DEFAULT '';"""
                     })
            {
                try
                {
                    await db.Database.ExecuteSqlRawAsync(sql, cancellationToken);
                }
                catch
                {
                    // column exists
                }
            }

            return;
        }

        await db.Database.ExecuteSqlRawAsync(
            """
            IF OBJECT_ID(N'[SystemSettings]', N'U') IS NULL
            BEGIN
              CREATE TABLE [SystemSettings] (
                [Id] uniqueidentifier NOT NULL CONSTRAINT [PK_SystemSettings] PRIMARY KEY,
                [OrganizationName] nvarchar(200) NOT NULL,
                [SupportEmail] nvarchar(256) NOT NULL,
                [DefaultCulture] nvarchar(10) NOT NULL,
                [MaxFailedLoginAttempts] int NOT NULL,
                [LockoutMinutes] int NOT NULL,
                [UpdatedAt] datetimeoffset NOT NULL
              );
            END
            """,
            cancellationToken);

        await db.Database.ExecuteSqlRawAsync(
            """
            IF COL_LENGTH('SystemSettings', 'ProductTitle') IS NULL
              ALTER TABLE [SystemSettings] ADD [ProductTitle] nvarchar(200) NOT NULL CONSTRAINT DF_SystemSettings_ProductTitle DEFAULT N'Customer Support CRM';
            IF COL_LENGTH('SystemSettings', 'PrimaryColor') IS NULL
              ALTER TABLE [SystemSettings] ADD [PrimaryColor] nvarchar(32) NOT NULL CONSTRAINT DF_SystemSettings_PrimaryColor DEFAULT N'#2563eb';
            IF COL_LENGTH('SystemSettings', 'LogoUrl') IS NULL
              ALTER TABLE [SystemSettings] ADD [LogoUrl] nvarchar(500) NOT NULL CONSTRAINT DF_SystemSettings_LogoUrl DEFAULT N'/brand/azm-squad.png';
            IF COL_LENGTH('SystemSettings', 'ErpWebhookUrl') IS NULL
              ALTER TABLE [SystemSettings] ADD [ErpWebhookUrl] nvarchar(500) NOT NULL CONSTRAINT DF_SystemSettings_ErpWebhookUrl DEFAULT N'';
            IF COL_LENGTH('SystemSettings', 'ErpWebhookAuthHeader') IS NULL
              ALTER TABLE [SystemSettings] ADD [ErpWebhookAuthHeader] nvarchar(500) NOT NULL CONSTRAINT DF_SystemSettings_ErpWebhookAuthHeader DEFAULT N'';
            """,
            cancellationToken);
    }

    /// <summary>SDD CRM-035 — EnsureCreated will not add tables to an existing DB.</summary>
    private async Task EnsurePermissionDefinitionsTableAsync(CancellationToken cancellationToken)
    {
        if (db.Database.IsSqlite())
        {
            await db.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE IF NOT EXISTS "PermissionDefinitions" (
                    "Name" TEXT NOT NULL CONSTRAINT "PK_PermissionDefinitions" PRIMARY KEY,
                    "Description" TEXT NOT NULL,
                    "CreatedAt" TEXT NOT NULL
                );
                """,
                cancellationToken);
            return;
        }

        await db.Database.ExecuteSqlRawAsync(
            """
            IF OBJECT_ID(N'[PermissionDefinitions]', N'U') IS NULL
            BEGIN
              CREATE TABLE [PermissionDefinitions] (
                [Name] nvarchar(120) NOT NULL CONSTRAINT [PK_PermissionDefinitions] PRIMARY KEY,
                [Description] nvarchar(400) NOT NULL,
                [CreatedAt] datetimeoffset NOT NULL
              );
            END
            """,
            cancellationToken);
    }

    /// <summary>SDD CRM-043 — departments / branches tables.</summary>
    private async Task EnsureDepartmentsTablesAsync(CancellationToken cancellationToken)
    {
        if (db.Database.IsSqlite())
        {
            await db.Database.ExecuteSqlRawAsync(
                """
                CREATE TABLE IF NOT EXISTS "Departments" (
                    "Id" TEXT NOT NULL CONSTRAINT "PK_Departments" PRIMARY KEY,
                    "Name" TEXT NOT NULL,
                    "CreatedAt" TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS "Branches" (
                    "Id" TEXT NOT NULL CONSTRAINT "PK_Branches" PRIMARY KEY,
                    "DepartmentId" TEXT NOT NULL,
                    "Name" TEXT NOT NULL,
                    "CreatedAt" TEXT NOT NULL
                );
                """,
                cancellationToken);
            return;
        }

        await db.Database.ExecuteSqlRawAsync(
            """
            IF OBJECT_ID(N'[Departments]', N'U') IS NULL
            BEGIN
              CREATE TABLE [Departments] (
                [Id] uniqueidentifier NOT NULL CONSTRAINT [PK_Departments] PRIMARY KEY,
                [Name] nvarchar(200) NOT NULL,
                [CreatedAt] datetimeoffset NOT NULL
              );
            END
            IF OBJECT_ID(N'[Branches]', N'U') IS NULL
            BEGIN
              CREATE TABLE [Branches] (
                [Id] uniqueidentifier NOT NULL CONSTRAINT [PK_Branches] PRIMARY KEY,
                [DepartmentId] uniqueidentifier NOT NULL,
                [Name] nvarchar(200) NOT NULL,
                [CreatedAt] datetimeoffset NOT NULL
              );
            END
            """,
            cancellationToken);
    }

    /// <summary>SDD CRM-043 — optional org columns on AspNetUsers.</summary>
    private async Task EnsureUserOrgColumnsAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (db.Database.IsSqlite())
            {
                await db.Database.ExecuteSqlRawAsync(
                    """ALTER TABLE "AspNetUsers" ADD COLUMN "DepartmentId" TEXT NULL;""",
                    cancellationToken);
            }
            else
            {
                await db.Database.ExecuteSqlRawAsync(
                    """
                    IF COL_LENGTH('AspNetUsers', 'DepartmentId') IS NULL
                      ALTER TABLE [AspNetUsers] ADD [DepartmentId] uniqueidentifier NULL;
                    IF COL_LENGTH('AspNetUsers', 'BranchId') IS NULL
                      ALTER TABLE [AspNetUsers] ADD [BranchId] uniqueidentifier NULL;
                    """,
                    cancellationToken);
            }
        }
        catch
        {
            // column may already exist
        }

        if (db.Database.IsSqlite())
        {
            try
            {
                await db.Database.ExecuteSqlRawAsync(
                    """ALTER TABLE "AspNetUsers" ADD COLUMN "BranchId" TEXT NULL;""",
                    cancellationToken);
            }
            catch
            {
                // already exists
            }
        }
    }

    /// <summary>SDD CRM-043</summary>
    public async Task<IReadOnlyList<DepartmentDto>> ListDepartmentsAsync(CancellationToken ct = default)
    {
        return await db.Departments.AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto(d.Id.ToString(), d.Name))
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<BranchDto>> ListBranchesAsync(Guid? departmentId, CancellationToken ct = default)
    {
        var q = db.Branches.AsNoTracking().AsQueryable();
        if (departmentId is { } id)
        {
            q = q.Where(b => b.DepartmentId == id);
        }

        return await q.OrderBy(b => b.Name)
            .Select(b => new BranchDto(b.Id.ToString(), b.DepartmentId.ToString(), b.Name))
            .ToListAsync(ct);
    }

    public async Task<(DepartmentDto? Dept, string? Error)> CreateDepartmentAsync(string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return (null, "Name is required.");
        }

        var row = new Department
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        db.Departments.Add(row);
        await db.SaveChangesAsync(ct);
        return (new DepartmentDto(row.Id.ToString(), row.Name), null);
    }

    public async Task<(BranchDto? Branch, string? Error)> CreateBranchAsync(
        Guid departmentId,
        string name,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return (null, "Name is required.");
        }

        if (!await db.Departments.AnyAsync(d => d.Id == departmentId, ct))
        {
            return (null, "Department not found.");
        }

        var row = new Branch
        {
            Id = Guid.NewGuid(),
            DepartmentId = departmentId,
            Name = name.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        db.Branches.Add(row);
        await db.SaveChangesAsync(ct);
        return (new BranchDto(row.Id.ToString(), row.DepartmentId.ToString(), row.Name), null);
    }

    public async Task<string?> AssignUserOrgAsync(
        Guid userId,
        Guid? departmentId,
        Guid? branchId,
        CancellationToken ct = default)
    {
        var entity = await users.FindByIdAsync(userId.ToString());
        if (entity is null)
        {
            return "User not found.";
        }

        if (departmentId is { } d && !await db.Departments.AnyAsync(x => x.Id == d, ct))
        {
            return "Department not found.";
        }

        if (branchId is { } b)
        {
            var branch = await db.Branches.AsNoTracking().FirstOrDefaultAsync(x => x.Id == b, ct);
            if (branch is null)
            {
                return "Branch not found.";
            }

            if (departmentId is { } dept && branch.DepartmentId != dept)
            {
                return "Branch does not belong to department.";
            }

            departmentId ??= branch.DepartmentId;
        }

        entity.DepartmentId = departmentId;
        entity.BranchId = branchId;
        await users.UpdateAsync(entity);
        return null;
    }

    public async Task<IReadOnlyList<UserSummaryDto>> ListUserSummariesAsync(string? q, CancellationToken ct = default)
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
        var result = new List<UserSummaryDto>();
        foreach (var user in list)
        {
            var roleList = await users.GetRolesAsync(user);
            var role = roleList.FirstOrDefault() ?? RoleNames.Agent;
            result.Add(new UserSummaryDto(
                user.Id.ToString(),
                user.Email ?? "",
                user.DisplayName,
                role,
                user.IsActive,
                user.DepartmentId?.ToString(),
                user.BranchId?.ToString()));
        }

        return result;
    }

    public async Task<Domain.SystemSettings> GetOrCreateSettingsAsync(CancellationToken ct = default)
    {
        var row = await db.SystemSettings.FirstOrDefaultAsync(s => s.Id == Domain.SystemSettings.SingletonId, ct);
        if (row is not null)
        {
            return row;
        }

        row = new Domain.SystemSettings { Id = Domain.SystemSettings.SingletonId, UpdatedAt = DateTimeOffset.UtcNow };
        db.SystemSettings.Add(row);
        await db.SaveChangesAsync(ct);
        return row;
    }

    public async Task<Domain.SystemSettings> UpdateSettingsAsync(
        string organizationName,
        string supportEmail,
        string defaultCulture,
        int maxFailedLoginAttempts,
        int lockoutMinutes,
        CancellationToken ct = default,
        string? productTitle = null,
        string? primaryColor = null,
        string? logoUrl = null,
        string? erpWebhookUrl = null,
        string? erpWebhookAuthHeader = null)
    {
        var row = await GetOrCreateSettingsAsync(ct);
        row.OrganizationName = organizationName.Trim();
        row.SupportEmail = supportEmail.Trim().ToLowerInvariant();
        row.DefaultCulture = defaultCulture.Trim().ToLowerInvariant();
        row.MaxFailedLoginAttempts = maxFailedLoginAttempts;
        row.LockoutMinutes = lockoutMinutes;
        if (productTitle is not null)
        {
            row.ProductTitle = string.IsNullOrWhiteSpace(productTitle) ? "Customer Support CRM" : productTitle.Trim();
        }

        if (primaryColor is not null)
        {
            row.PrimaryColor = string.IsNullOrWhiteSpace(primaryColor) ? "#2563eb" : primaryColor.Trim();
        }

        if (logoUrl is not null)
        {
            row.LogoUrl = string.IsNullOrWhiteSpace(logoUrl) ? "/brand/azm-squad.png" : logoUrl.Trim();
        }

        if (erpWebhookUrl is not null)
        {
            row.ErpWebhookUrl = erpWebhookUrl.Trim();
        }

        if (erpWebhookAuthHeader is not null)
        {
            row.ErpWebhookAuthHeader = erpWebhookAuthHeader.Trim();
        }

        row.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return row;
    }

    public async Task AppendAuditAsync(
        string action,
        bool success,
        Guid? actorUserId,
        string? actorEmail,
        Guid? targetUserId,
        string? targetEmail,
        string? detail,
        CancellationToken ct = default,
        string? service = null)
    {
        db.AuditLogs.Add(new AuditLogEntry
        {
            Id = Guid.NewGuid(),
            OccurredAt = DateTimeOffset.UtcNow,
            Action = action,
            ActorUserId = actorUserId,
            ActorEmail = actorEmail,
            TargetUserId = targetUserId,
            TargetEmail = targetEmail,
            Detail = detail,
            Success = success,
            Service = string.IsNullOrWhiteSpace(service) ? AuditServices.Identity : service.Trim()
        });
        await db.SaveChangesAsync(ct);
    }

    public async Task<(IReadOnlyList<AuditLogEntry> Items, int Total)> SearchAuditPageAsync(
        string? q,
        string? service,
        int skip,
        int take,
        CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 100);
        skip = Math.Max(0, skip);
        var query = db.AuditLogs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLowerInvariant();
            query = query.Where(e =>
                e.Action.ToLower().Contains(term) ||
                e.Service.ToLower().Contains(term) ||
                (e.ActorEmail != null && e.ActorEmail.ToLower().Contains(term)) ||
                (e.TargetEmail != null && e.TargetEmail.ToLower().Contains(term)) ||
                (e.Detail != null && e.Detail.ToLower().Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(service))
        {
            var svc = service.Trim().ToLowerInvariant();
            query = query.Where(e => e.Service.ToLower() == svc);
        }

        // Sqlite cannot ORDER BY DateTimeOffset reliably — materialize then page in memory (demo scale).
        var rows = await query.ToListAsync(ct);
        var ordered = rows.OrderByDescending(e => e.OccurredAt).ToList();
        var total = ordered.Count;
        var page = ordered.Skip(skip).Take(take).ToList();
        return (page, total);
    }

    public async Task<IReadOnlyList<AuditLogEntry>> SearchAuditAsync(
        string? q,
        int take,
        CancellationToken ct = default)
    {
        var (items, _) = await SearchAuditPageAsync(q, null, 0, take, ct);
        return items;
    }

    public async Task<int> CountAuditAsync(CancellationToken ct = default)
        => await db.AuditLogs.CountAsync(ct);

    public async Task<AuditLogEntry?> GetAuditByIdAsync(Guid id, CancellationToken ct = default)
        => await db.AuditLogs.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);

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
        CancellationToken ct = default,
        Guid? departmentId = null,
        Guid? branchId = null)
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
            CreatedAt = DateTimeOffset.UtcNow,
            DepartmentId = departmentId,
            BranchId = branchId
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

        var settings = await GetOrCreateSettingsAsync(ct);
        entity.AccessFailedCount++;
        if (entity.AccessFailedCount >= settings.MaxFailedLoginAttempts)
        {
            await users.SetLockoutEndDateAsync(entity, now.AddMinutes(settings.LockoutMinutes));
            await users.ResetAccessFailedCountAsync(entity);
            return;
        }

        await users.UpdateAsync(entity);
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

    public async Task<IReadOnlyList<string>> ListPermissionNamesAsync(CancellationToken ct = default)
    {
        return await db.PermissionDefinitions.AsNoTracking()
            .OrderBy(p => p.Name)
            .Select(p => p.Name)
            .ToListAsync(ct);
    }

    public async Task<(PermissionDefinition? Row, string? Error)> CreatePermissionAsync(
        string name,
        string? description,
        CancellationToken ct = default)
    {
        var normalized = NormalizePermissionName(name);
        if (normalized is null)
        {
            return (null, "Permission name must look like 'area.action' (letters, digits, ., *, -).");
        }

        if (await db.PermissionDefinitions.AnyAsync(p => p.Name == normalized, ct))
        {
            return (null, "Permission already exists.");
        }

        var row = new PermissionDefinition
        {
            Name = normalized,
            Description = (description ?? "").Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
        db.PermissionDefinitions.Add(row);
        await db.SaveChangesAsync(ct);
        return (row, null);
    }

    public async Task<(PermissionDefinition? Row, string? Error)> UpdatePermissionAsync(
        string currentName,
        string newName,
        string? description,
        CancellationToken ct = default)
    {
        var from = NormalizePermissionName(currentName);
        var to = NormalizePermissionName(newName);
        if (from is null || to is null)
        {
            return (null, "Permission name must look like 'area.action' (letters, digits, ., *, -).");
        }

        var row = await db.PermissionDefinitions.FirstOrDefaultAsync(p => p.Name == from, ct);
        if (row is null)
        {
            return (null, "Permission not found.");
        }

        if (!string.Equals(from, to, StringComparison.OrdinalIgnoreCase)
            && await db.PermissionDefinitions.AnyAsync(p => p.Name == to, ct))
        {
            return (null, "A permission with the new name already exists.");
        }

        row.Description = (description ?? row.Description).Trim();
        if (!string.Equals(from, to, StringComparison.Ordinal))
        {
            db.PermissionDefinitions.Remove(row);
            var renamed = new PermissionDefinition
            {
                Name = to,
                Description = row.Description,
                CreatedAt = row.CreatedAt
            };
            db.PermissionDefinitions.Add(renamed);
            await RenamePermissionClaimsAsync(from, to, ct);
            await db.SaveChangesAsync(ct);
            return (renamed, null);
        }

        await db.SaveChangesAsync(ct);
        return (row, null);
    }

    public async Task<string?> DeletePermissionAsync(string name, CancellationToken ct = default)
    {
        var normalized = NormalizePermissionName(name);
        if (normalized is null)
        {
            return "Permission name is invalid.";
        }

        var row = await db.PermissionDefinitions.FirstOrDefaultAsync(p => p.Name == normalized, ct);
        if (row is null)
        {
            return "Permission not found.";
        }

        db.PermissionDefinitions.Remove(row);
        await RemovePermissionClaimsAsync(normalized, ct);
        await db.SaveChangesAsync(ct);
        return null;
    }

    public async Task<(Role? Role, string? Error)> SetRolePermissionsAsync(
        string roleName,
        IEnumerable<string> permissions,
        CancellationToken ct = default)
    {
        var role = await roles.FindByNameAsync(roleName.Trim());
        if (role is null)
        {
            return (null, "Role not found.");
        }

        var desired = permissions
            .Select(NormalizePermissionName)
            .Where(p => p is not null)
            .Cast<string>()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(p => p)
            .ToList();

        var catalog = await ListPermissionNamesAsync(ct);
        var unknown = desired.Where(p => !catalog.Contains(p, StringComparer.OrdinalIgnoreCase)).ToList();
        if (unknown.Count > 0)
        {
            return (null, "Unknown permissions: " + string.Join(", ", unknown));
        }

        var claims = await roles.GetClaimsAsync(role);
        foreach (var claim in claims.Where(c => c.Type == IdentityDataSeeder.PermissionClaimType))
        {
            await roles.RemoveClaimAsync(role, claim);
        }

        foreach (var permission in desired)
        {
            await roles.AddClaimAsync(
                role,
                new System.Security.Claims.Claim(IdentityDataSeeder.PermissionClaimType, permission));
        }

        return (await ToRoleAsync(role), null);
    }

    private async Task RenamePermissionClaimsAsync(string from, string to, CancellationToken ct)
    {
        var allRoles = await roles.Roles.ToListAsync(ct);
        foreach (var role in allRoles)
        {
            var claims = await roles.GetClaimsAsync(role);
            var match = claims.FirstOrDefault(c =>
                c.Type == IdentityDataSeeder.PermissionClaimType
                && string.Equals(c.Value, from, StringComparison.OrdinalIgnoreCase));
            if (match is null)
            {
                continue;
            }

            await roles.RemoveClaimAsync(role, match);
            await roles.AddClaimAsync(
                role,
                new System.Security.Claims.Claim(IdentityDataSeeder.PermissionClaimType, to));
        }
    }

    private async Task RemovePermissionClaimsAsync(string permission, CancellationToken ct)
    {
        var allRoles = await roles.Roles.ToListAsync(ct);
        foreach (var role in allRoles)
        {
            var claims = await roles.GetClaimsAsync(role);
            foreach (var claim in claims.Where(c =>
                         c.Type == IdentityDataSeeder.PermissionClaimType
                         && string.Equals(c.Value, permission, StringComparison.OrdinalIgnoreCase)))
            {
                await roles.RemoveClaimAsync(role, claim);
            }
        }
    }

    private static string? NormalizePermissionName(string? raw)
    {
        var name = raw?.Trim().ToLowerInvariant() ?? "";
        if (name.Length is < 3 or > 120)
        {
            return null;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-z][a-z0-9._*-]*$"))
        {
            return null;
        }

        return name;
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

    /// <summary>SDD CRM-046 — ASP.NET Identity password-reset token (null if no active user).</summary>
    public async Task<string?> CreatePasswordResetTokenAsync(string email, CancellationToken ct = default)
    {
        email = email.Trim().ToLowerInvariant();
        var entity = await users.FindByEmailAsync(email);
        if (entity is null || !entity.IsActive)
        {
            return null;
        }

        return await users.GeneratePasswordResetTokenAsync(entity);
    }

    /// <summary>SDD CRM-046 — consume reset token; returns error message or null on success.</summary>
    public async Task<string?> ResetPasswordWithTokenAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken ct = default)
    {
        email = email.Trim().ToLowerInvariant();
        var entity = await users.FindByEmailAsync(email);
        if (entity is null || !entity.IsActive)
        {
            return "Invalid or expired reset token.";
        }

        var result = await users.ResetPasswordAsync(entity, token, newPassword);
        if (!result.Succeeded)
        {
            var desc = string.Join("; ", result.Errors.Select(e => e.Description));
            if (desc.Contains("Invalid token", StringComparison.OrdinalIgnoreCase) ||
                desc.Contains("InvalidToken", StringComparison.OrdinalIgnoreCase))
            {
                return "Invalid or expired reset token.";
            }

            return string.IsNullOrWhiteSpace(desc) ? "Password reset failed." : desc;
        }

        entity.LockoutEnd = null;
        await users.ResetAccessFailedCountAsync(entity);
        await users.UpdateAsync(entity);
        return null;
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
