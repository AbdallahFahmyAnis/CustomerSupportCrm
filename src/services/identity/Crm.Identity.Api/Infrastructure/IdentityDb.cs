using System.Data.Common;
using Crm.BuildingBlocks.Identity;
using Crm.Identity.Api.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace Crm.Identity.Api.Infrastructure;

/// <summary>
/// SDD CRM-035 / CRM-037 / specs/006-data-platform —
/// SQL Server when ConnectionStrings:Identity (or Provider=SqlServer) is set; otherwise SQLite.
/// </summary>
public sealed class IdentityDb
{
    private readonly string _connectionString;
    private readonly bool _sqlServer;

    public IdentityDb(IWebHostEnvironment env, IConfiguration config)
    {
        var provider = (config["CRM_IDENTITY_PROVIDER"]
                        ?? config["Identity:Provider"]
                        ?? string.Empty).Trim();
        var sqlCs = config.GetConnectionString("Identity");
        _sqlServer = provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
                     || !string.IsNullOrWhiteSpace(sqlCs)
                        && !provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase);

        if (_sqlServer)
        {
            if (string.IsNullOrWhiteSpace(sqlCs))
            {
                throw new InvalidOperationException(
                    "Identity Provider=SqlServer requires ConnectionStrings:Identity.");
            }

            _connectionString = sqlCs;
        }
        else
        {
            var dataRoot = Path.GetFullPath(config["Identity:DataPath"] ?? Path.Combine(env.ContentRootPath, "data"));
            Directory.CreateDirectory(dataRoot);
            _connectionString = $"Data Source={Path.Combine(dataRoot, "identity.db")}";
        }
    }

    public void EnsureSchema()
    {
        if (_sqlServer)
        {
            EnsureSqlServerDatabase();
        }

        using var connection = Open();
        using (var command = connection.CreateCommand())
        {
            command.CommandText = _sqlServer ? SqlServerSchemaSql : SqliteSchemaSql;
            command.ExecuteNonQuery();
        }

        if (!_sqlServer)
        {
            TryAddSqliteColumn(connection, "Users", "FailedLoginCount", "INTEGER NOT NULL DEFAULT 0");
            TryAddSqliteColumn(connection, "Users", "LockoutUntil", "TEXT NULL");
        }
    }

    private void EnsureSqlServerDatabase()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString);
        var database = builder.InitialCatalog;
        if (string.IsNullOrWhiteSpace(database))
        {
            return;
        }

        if (!database.All(c => char.IsLetterOrDigit(c) || c is '_' or '-'))
        {
            throw new InvalidOperationException("Identity SQL catalog name is invalid.");
        }

        builder.InitialCatalog = "master";
        using var connection = new SqlConnection(builder.ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            $"""
             IF DB_ID(N'{database}') IS NULL
             BEGIN
               CREATE DATABASE [{database}];
             END
             """;
        command.ExecuteNonQuery();
    }

    private static void TryAddSqliteColumn(DbConnection connection, string table, string column, string definition)
    {
        try
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {definition}";
            command.ExecuteNonQuery();
        }
        catch
        {
            // column already exists
        }
    }

    public void SeedIfEmpty()
    {
        try
        {
            using var connection = Open();
            using var countCmd = connection.CreateCommand();
            countCmd.CommandText = "SELECT COUNT(1) FROM Roles";
            if (Convert.ToInt64(countCmd.ExecuteScalar()) == 0)
            {
                UpsertRole(Role.Define(RoleNames.Admin, "Full administration",
                [
                    PermissionCatalog.UsersManage,
                    PermissionCatalog.RolesView,
                    PermissionCatalog.TicketsAll,
                    PermissionCatalog.CustomersAll
                ]));
                UpsertRole(Role.Define(RoleNames.Lead, "Team lead",
                [
                    PermissionCatalog.TicketsAll,
                    PermissionCatalog.TicketsAssign,
                    PermissionCatalog.CustomersAll
                ]));
                UpsertRole(Role.Define(RoleNames.Agent, "Support agent",
                [
                    PermissionCatalog.TicketsWork,
                    PermissionCatalog.CustomersRead
                ]));
            }

            using var userCount = connection.CreateCommand();
            userCount.CommandText = "SELECT COUNT(1) FROM Users";
            if (Convert.ToInt64(userCount.ExecuteScalar()) > 0)
            {
                return;
            }

            Insert(UserAccount.Register(
                DevUsers.AgentEmail,
                DevUsers.AgentName,
                DevUsers.Password,
                RoleNames.Agent,
                Guid.Parse(DevUsers.AgentId)));
            Insert(UserAccount.Register(
                "admin@crm.local",
                "Demo Admin",
                DevUsers.Password,
                RoleNames.Admin,
                Guid.Parse("33333333-3333-3333-3333-333333333333")));
            Insert(UserAccount.Register(
                "lead@crm.local",
                "Lead Agent",
                DevUsers.Password,
                RoleNames.Lead,
                Guid.Parse("22222222-2222-2222-2222-222222222222")));
        }
        catch
        {
            // never brick startup
        }
    }

    public IReadOnlyList<UserAccount> SearchUsers(string? q)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        if (string.IsNullOrWhiteSpace(q))
        {
            command.CommandText = _sqlServer
                ? """
                  SELECT TOP 100 Id, Email, DisplayName, PasswordHash, Role, IsActive, FailedLoginCount, LockoutUntil, CreatedAt
                  FROM Users ORDER BY DisplayName
                  """
                : """
                  SELECT Id, Email, DisplayName, PasswordHash, Role, IsActive, FailedLoginCount, LockoutUntil, CreatedAt
                  FROM Users ORDER BY DisplayName LIMIT 100
                  """;
        }
        else
        {
            command.CommandText = _sqlServer
                ? """
                  SELECT TOP 100 Id, Email, DisplayName, PasswordHash, Role, IsActive, FailedLoginCount, LockoutUntil, CreatedAt
                  FROM Users
                  WHERE Email LIKE @q OR DisplayName LIKE @q OR Role LIKE @q
                  ORDER BY DisplayName
                  """
                : """
                  SELECT Id, Email, DisplayName, PasswordHash, Role, IsActive, FailedLoginCount, LockoutUntil, CreatedAt
                  FROM Users
                  WHERE Email LIKE @q OR DisplayName LIKE @q OR Role LIKE @q
                  ORDER BY DisplayName LIMIT 100
                  """;
            AddParam(command, "@q", $"%{q.Trim()}%");
        }

        var list = new List<UserAccount>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(ReadUser(reader));
        }

        return list;
    }

    public UserAccount? GetUser(Guid id)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, Email, DisplayName, PasswordHash, Role, IsActive, FailedLoginCount, LockoutUntil, CreatedAt
            FROM Users WHERE Id = @id
            """;
        AddParam(command, "@id", id.ToString());
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadUser(reader) : null;
    }

    public UserAccount? FindByEmail(string email)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, Email, DisplayName, PasswordHash, Role, IsActive, FailedLoginCount, LockoutUntil, CreatedAt
            FROM Users WHERE Email = @email
            """;
        AddParam(command, "@email", email.Trim().ToLowerInvariant());
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadUser(reader) : null;
    }

    public void Insert(UserAccount user)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO Users (Id, Email, DisplayName, PasswordHash, Role, IsActive, FailedLoginCount, LockoutUntil, CreatedAt)
            VALUES (@id, @email, @name, @hash, @role, @active, @fails, @lockout, @created)
            """;
        BindUser(command, user);
        command.ExecuteNonQuery();
    }

    public void Update(UserAccount user)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE Users SET DisplayName = @name, PasswordHash = @hash, Role = @role, IsActive = @active,
              FailedLoginCount = @fails, LockoutUntil = @lockout
            WHERE Id = @id
            """;
        AddParam(command, "@id", user.Id.ToString());
        AddParam(command, "@name", user.DisplayName);
        AddParam(command, "@hash", user.PasswordHash);
        AddParam(command, "@role", user.Role);
        AddParam(command, "@active", user.IsActive ? 1 : 0);
        AddParam(command, "@fails", user.FailedLoginCount);
        AddParam(command, "@lockout", (object?)user.LockoutUntil?.ToString("O") ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public void InsertRefreshToken(StoredRefreshToken token)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO RefreshTokens (Id, UserId, TokenHash, ExpiresAt, CreatedAt, RevokedAt, ReplacedByTokenId)
            VALUES (@id, @uid, @hash, @exp, @created, @revoked, @replaced)
            """;
        AddParam(command, "@id", token.Id.ToString());
        AddParam(command, "@uid", token.UserId.ToString());
        AddParam(command, "@hash", token.TokenHash);
        AddParam(command, "@exp", token.ExpiresAt.ToString("O"));
        AddParam(command, "@created", token.CreatedAt.ToString("O"));
        AddParam(command, "@revoked", (object?)token.RevokedAt?.ToString("O") ?? DBNull.Value);
        AddParam(command, "@replaced", (object?)token.ReplacedByTokenId?.ToString() ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public StoredRefreshToken? FindRefreshTokenByHash(string tokenHash)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, UserId, TokenHash, ExpiresAt, CreatedAt, RevokedAt, ReplacedByTokenId
            FROM RefreshTokens WHERE TokenHash = @hash
            """;
        AddParam(command, "@hash", tokenHash);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new StoredRefreshToken
        {
            Id = Guid.Parse(reader.GetString(0)),
            UserId = Guid.Parse(reader.GetString(1)),
            TokenHash = reader.GetString(2),
            ExpiresAt = DateTimeOffset.Parse(reader.GetString(3)),
            CreatedAt = DateTimeOffset.Parse(reader.GetString(4)),
            RevokedAt = reader.IsDBNull(5) ? null : DateTimeOffset.Parse(reader.GetString(5)),
            ReplacedByTokenId = reader.IsDBNull(6) ? null : Guid.Parse(reader.GetString(6))
        };
    }

    public void RevokeRefreshToken(Guid id, DateTimeOffset revokedAt, Guid? replacedBy = null)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE RefreshTokens SET RevokedAt = @revoked, ReplacedByTokenId = @replaced
            WHERE Id = @id AND RevokedAt IS NULL
            """;
        AddParam(command, "@id", id.ToString());
        AddParam(command, "@revoked", revokedAt.ToString("O"));
        AddParam(command, "@replaced", (object?)replacedBy?.ToString() ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public void RevokeAllRefreshTokensForUser(Guid userId, DateTimeOffset revokedAt)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            UPDATE RefreshTokens SET RevokedAt = @revoked
            WHERE UserId = @uid AND RevokedAt IS NULL
            """;
        AddParam(command, "@uid", userId.ToString());
        AddParam(command, "@revoked", revokedAt.ToString("O"));
        command.ExecuteNonQuery();
    }

    public void RevokeAccessJti(string jti, Guid userId, DateTimeOffset expiresAt, DateTimeOffset revokedAt)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = _sqlServer
            ? """
              MERGE RevokedAccessTokens AS t
              USING (SELECT @jti AS Jti) AS s ON t.Jti = s.Jti
              WHEN MATCHED THEN UPDATE SET UserId = @uid, ExpiresAt = @exp, RevokedAt = @revoked
              WHEN NOT MATCHED THEN INSERT (Jti, UserId, ExpiresAt, RevokedAt) VALUES (@jti, @uid, @exp, @revoked);
              """
            : """
              INSERT OR REPLACE INTO RevokedAccessTokens (Jti, UserId, ExpiresAt, RevokedAt)
              VALUES (@jti, @uid, @exp, @revoked)
              """;
        AddParam(command, "@jti", jti);
        AddParam(command, "@uid", userId.ToString());
        AddParam(command, "@exp", expiresAt.ToString("O"));
        AddParam(command, "@revoked", revokedAt.ToString("O"));
        command.ExecuteNonQuery();
    }

    public bool IsAccessJtiRevoked(string jti)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = _sqlServer
            ? "SELECT TOP 1 1 FROM RevokedAccessTokens WHERE Jti = @jti"
            : "SELECT 1 FROM RevokedAccessTokens WHERE Jti = @jti LIMIT 1";
        AddParam(command, "@jti", jti);
        return command.ExecuteScalar() is not null;
    }

    public IReadOnlyList<Role> ListRoles()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Name, Description FROM Roles ORDER BY Name";
        var roles = new List<Role>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var name = reader.GetString(0);
            var description = reader.GetString(1);
            roles.Add(Role.Rehydrate(name, description, LoadPermissions(connection, name)));
        }

        return roles;
    }

    public Role? GetRole(string name)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Name, Description FROM Roles WHERE Name = @name";
        AddParam(command, "@name", name);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var roleName = reader.GetString(0);
        var description = reader.GetString(1);
        reader.Close();
        return Role.Rehydrate(roleName, description, LoadPermissions(connection, roleName));
    }

    public void UpsertRole(Role role)
    {
        using var connection = Open();
        using var tx = connection.BeginTransaction();
        using (var command = connection.CreateCommand())
        {
            command.Transaction = tx;
            command.CommandText = _sqlServer
                ? """
                  MERGE Roles AS t
                  USING (SELECT @name AS Name) AS s ON t.Name = s.Name
                  WHEN MATCHED THEN UPDATE SET Description = @desc
                  WHEN NOT MATCHED THEN INSERT (Name, Description) VALUES (@name, @desc);
                  """
                : """
                  INSERT INTO Roles (Name, Description) VALUES (@name, @desc)
                  ON CONFLICT(Name) DO UPDATE SET Description = excluded.Description
                  """;
            AddParam(command, "@name", role.Name);
            AddParam(command, "@desc", role.Description);
            command.ExecuteNonQuery();
        }

        using (var clear = connection.CreateCommand())
        {
            clear.Transaction = tx;
            clear.CommandText = "DELETE FROM RolePermissions WHERE RoleName = @name";
            AddParam(clear, "@name", role.Name);
            clear.ExecuteNonQuery();
        }

        foreach (var permission in role.Permissions)
        {
            using var insert = connection.CreateCommand();
            insert.Transaction = tx;
            insert.CommandText =
                """
                INSERT INTO RolePermissions (RoleName, Permission) VALUES (@name, @perm)
                """;
            AddParam(insert, "@name", role.Name);
            AddParam(insert, "@perm", permission);
            insert.ExecuteNonQuery();
        }

        tx.Commit();
    }

    private List<string> LoadPermissions(DbConnection connection, string roleName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Permission FROM RolePermissions WHERE RoleName = @name ORDER BY Permission";
        AddParam(command, "@name", roleName);
        var list = new List<string>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(reader.GetString(0));
        }

        return list;
    }

    private static void BindUser(DbCommand command, UserAccount user)
    {
        AddParam(command, "@id", user.Id.ToString());
        AddParam(command, "@email", user.Email);
        AddParam(command, "@name", user.DisplayName);
        AddParam(command, "@hash", user.PasswordHash);
        AddParam(command, "@role", user.Role);
        AddParam(command, "@active", user.IsActive ? 1 : 0);
        AddParam(command, "@fails", user.FailedLoginCount);
        AddParam(command, "@lockout", (object?)user.LockoutUntil?.ToString("O") ?? DBNull.Value);
        AddParam(command, "@created", user.CreatedAt.ToString("O"));
    }

    private static UserAccount ReadUser(DbDataReader reader)
        => UserAccount.Rehydrate(
            Guid.Parse(reader.GetString(0)),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4),
            Convert.ToInt32(reader.GetValue(5)) == 1,
            reader.IsDBNull(6) ? 0 : Convert.ToInt32(reader.GetValue(6)),
            reader.IsDBNull(7) ? null : DateTimeOffset.Parse(reader.GetString(7)),
            DateTimeOffset.Parse(reader.GetString(8)));

    private static void AddParam(DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    private DbConnection Open()
    {
        DbConnection connection = _sqlServer
            ? new SqlConnection(_connectionString)
            : new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    private const string SqliteSchemaSql =
        """
        CREATE TABLE IF NOT EXISTS Users (
          Id TEXT PRIMARY KEY,
          Email TEXT NOT NULL UNIQUE,
          DisplayName TEXT NOT NULL,
          PasswordHash TEXT NOT NULL,
          Role TEXT NOT NULL,
          IsActive INTEGER NOT NULL,
          FailedLoginCount INTEGER NOT NULL DEFAULT 0,
          LockoutUntil TEXT NULL,
          CreatedAt TEXT NOT NULL
        );
        CREATE TABLE IF NOT EXISTS Roles (
          Name TEXT PRIMARY KEY,
          Description TEXT NOT NULL
        );
        CREATE TABLE IF NOT EXISTS RolePermissions (
          RoleName TEXT NOT NULL,
          Permission TEXT NOT NULL,
          PRIMARY KEY (RoleName, Permission),
          FOREIGN KEY (RoleName) REFERENCES Roles(Name)
        );
        CREATE TABLE IF NOT EXISTS RefreshTokens (
          Id TEXT PRIMARY KEY,
          UserId TEXT NOT NULL,
          TokenHash TEXT NOT NULL UNIQUE,
          ExpiresAt TEXT NOT NULL,
          CreatedAt TEXT NOT NULL,
          RevokedAt TEXT NULL,
          ReplacedByTokenId TEXT NULL
        );
        CREATE TABLE IF NOT EXISTS RevokedAccessTokens (
          Jti TEXT PRIMARY KEY,
          UserId TEXT NOT NULL,
          ExpiresAt TEXT NOT NULL,
          RevokedAt TEXT NOT NULL
        );
        """;

    private const string SqlServerSchemaSql =
        """
        IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
        CREATE TABLE dbo.Users (
          Id NVARCHAR(36) NOT NULL PRIMARY KEY,
          Email NVARCHAR(320) NOT NULL UNIQUE,
          DisplayName NVARCHAR(200) NOT NULL,
          PasswordHash NVARCHAR(500) NOT NULL,
          Role NVARCHAR(100) NOT NULL,
          IsActive INT NOT NULL,
          FailedLoginCount INT NOT NULL CONSTRAINT DF_Users_FailedLoginCount DEFAULT 0,
          LockoutUntil NVARCHAR(40) NULL,
          CreatedAt NVARCHAR(40) NOT NULL
        );
        IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
        CREATE TABLE dbo.Roles (
          Name NVARCHAR(100) NOT NULL PRIMARY KEY,
          Description NVARCHAR(500) NOT NULL
        );
        IF OBJECT_ID(N'dbo.RolePermissions', N'U') IS NULL
        CREATE TABLE dbo.RolePermissions (
          RoleName NVARCHAR(100) NOT NULL,
          Permission NVARCHAR(200) NOT NULL,
          PRIMARY KEY (RoleName, Permission),
          FOREIGN KEY (RoleName) REFERENCES dbo.Roles(Name)
        );
        IF OBJECT_ID(N'dbo.RefreshTokens', N'U') IS NULL
        CREATE TABLE dbo.RefreshTokens (
          Id NVARCHAR(36) NOT NULL PRIMARY KEY,
          UserId NVARCHAR(36) NOT NULL,
          TokenHash NVARCHAR(200) NOT NULL UNIQUE,
          ExpiresAt NVARCHAR(40) NOT NULL,
          CreatedAt NVARCHAR(40) NOT NULL,
          RevokedAt NVARCHAR(40) NULL,
          ReplacedByTokenId NVARCHAR(36) NULL
        );
        IF OBJECT_ID(N'dbo.RevokedAccessTokens', N'U') IS NULL
        CREATE TABLE dbo.RevokedAccessTokens (
          Jti NVARCHAR(100) NOT NULL PRIMARY KEY,
          UserId NVARCHAR(36) NOT NULL,
          ExpiresAt NVARCHAR(40) NOT NULL,
          RevokedAt NVARCHAR(40) NOT NULL
        );
        """;
}
