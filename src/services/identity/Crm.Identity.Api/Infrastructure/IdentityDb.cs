using Crm.BuildingBlocks.Identity;
using Crm.Identity.Api.Domain;
using Microsoft.Data.Sqlite;

namespace Crm.Identity.Api.Infrastructure;

/// <summary>SDD CRM-035 / specs/004-identity-admin — SQLite persistence.</summary>
public sealed class IdentityDb
{
    private readonly string _connectionString;

    public IdentityDb(IWebHostEnvironment env, IConfiguration config)
    {
        var dataRoot = Path.GetFullPath(config["Identity:DataPath"] ?? Path.Combine(env.ContentRootPath, "data"));
        Directory.CreateDirectory(dataRoot);
        _connectionString = $"Data Source={Path.Combine(dataRoot, "identity.db")}";
    }

    public void EnsureSchema()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Users (
              Id TEXT PRIMARY KEY,
              Email TEXT NOT NULL UNIQUE,
              DisplayName TEXT NOT NULL,
              PasswordHash TEXT NOT NULL,
              Role TEXT NOT NULL,
              IsActive INTEGER NOT NULL,
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
            """;
        command.ExecuteNonQuery();
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
            command.CommandText =
                """
                SELECT Id, Email, DisplayName, PasswordHash, Role, IsActive, CreatedAt
                FROM Users ORDER BY DisplayName LIMIT 100
                """;
        }
        else
        {
            command.CommandText =
                """
                SELECT Id, Email, DisplayName, PasswordHash, Role, IsActive, CreatedAt
                FROM Users
                WHERE Email LIKE $q OR DisplayName LIKE $q OR Role LIKE $q
                ORDER BY DisplayName LIMIT 100
                """;
            command.Parameters.AddWithValue("$q", $"%{q.Trim()}%");
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
            SELECT Id, Email, DisplayName, PasswordHash, Role, IsActive, CreatedAt
            FROM Users WHERE Id = $id
            """;
        command.Parameters.AddWithValue("$id", id.ToString());
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadUser(reader) : null;
    }

    public UserAccount? FindByEmail(string email)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT Id, Email, DisplayName, PasswordHash, Role, IsActive, CreatedAt
            FROM Users WHERE Email = $email
            """;
        command.Parameters.AddWithValue("$email", email.Trim().ToLowerInvariant());
        using var reader = command.ExecuteReader();
        return reader.Read() ? ReadUser(reader) : null;
    }

    public void Insert(UserAccount user)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT INTO Users (Id, Email, DisplayName, PasswordHash, Role, IsActive, CreatedAt)
            VALUES ($id, $email, $name, $hash, $role, $active, $created)
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
            UPDATE Users SET DisplayName = $name, PasswordHash = $hash, Role = $role, IsActive = $active
            WHERE Id = $id
            """;
        command.Parameters.AddWithValue("$id", user.Id.ToString());
        command.Parameters.AddWithValue("$name", user.DisplayName);
        command.Parameters.AddWithValue("$hash", user.PasswordHash);
        command.Parameters.AddWithValue("$role", user.Role);
        command.Parameters.AddWithValue("$active", user.IsActive ? 1 : 0);
        command.ExecuteNonQuery();
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
        command.CommandText = "SELECT Name, Description FROM Roles WHERE Name = $name";
        command.Parameters.AddWithValue("$name", name);
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
            command.CommandText =
                """
                INSERT INTO Roles (Name, Description) VALUES ($name, $desc)
                ON CONFLICT(Name) DO UPDATE SET Description = excluded.Description
                """;
            command.Parameters.AddWithValue("$name", role.Name);
            command.Parameters.AddWithValue("$desc", role.Description);
            command.ExecuteNonQuery();
        }

        using (var clear = connection.CreateCommand())
        {
            clear.Transaction = tx;
            clear.CommandText = "DELETE FROM RolePermissions WHERE RoleName = $name";
            clear.Parameters.AddWithValue("$name", role.Name);
            clear.ExecuteNonQuery();
        }

        foreach (var permission in role.Permissions)
        {
            using var insert = connection.CreateCommand();
            insert.Transaction = tx;
            insert.CommandText =
                """
                INSERT INTO RolePermissions (RoleName, Permission) VALUES ($name, $perm)
                """;
            insert.Parameters.AddWithValue("$name", role.Name);
            insert.Parameters.AddWithValue("$perm", permission);
            insert.ExecuteNonQuery();
        }

        tx.Commit();
    }

    private static List<string> LoadPermissions(SqliteConnection connection, string roleName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Permission FROM RolePermissions WHERE RoleName = $name ORDER BY Permission";
        command.Parameters.AddWithValue("$name", roleName);
        var list = new List<string>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            list.Add(reader.GetString(0));
        }

        return list;
    }

    private static void BindUser(SqliteCommand command, UserAccount user)
    {
        command.Parameters.AddWithValue("$id", user.Id.ToString());
        command.Parameters.AddWithValue("$email", user.Email);
        command.Parameters.AddWithValue("$name", user.DisplayName);
        command.Parameters.AddWithValue("$hash", user.PasswordHash);
        command.Parameters.AddWithValue("$role", user.Role);
        command.Parameters.AddWithValue("$active", user.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("$created", user.CreatedAt.ToString("O"));
    }

    private static UserAccount ReadUser(SqliteDataReader reader)
        => UserAccount.Rehydrate(
            Guid.Parse(reader.GetString(0)),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.GetInt64(5) == 1,
            DateTimeOffset.Parse(reader.GetString(6)));

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
