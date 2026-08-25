using System.Net;
using System.Net.Http.Json;
using Crm.Contracts.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Crm.Identity.Api.Tests;

public sealed class IdentityAdminTests : IClassFixture<IdentityApiFactory>
{
    private readonly HttpClient _client;

    public IdentityAdminTests(IdentityApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    [Trait("Story", "CRM-035")]
    [Trait("Story", "CRM-037")]
    public async Task Login_issues_access_and_refresh_tokens()
    {
        var agent = await _client.PostAsJsonAsync("/api/identity/token",
            new DevLoginRequest("agent@crm.local", "Crm!123"));
        agent.StatusCode.Should().Be(HttpStatusCode.OK);
        var tokens = await agent.Content.ReadFromJsonAsync<TokenResponseDto>();
        tokens!.User.Role.Should().Be("Agent");
        tokens.AccessToken.Should().NotBeNullOrWhiteSpace();
        tokens.RefreshToken.Should().NotBeNullOrWhiteSpace();

        var admin = await _client.PostAsJsonAsync("/api/identity/token",
            new DevLoginRequest("admin@crm.local", "Crm!123"));
        admin.StatusCode.Should().Be(HttpStatusCode.OK);
        (await admin.Content.ReadFromJsonAsync<TokenResponseDto>())!.User.Role.Should().Be("Admin");
    }

    [Fact]
    [Trait("Story", "CRM-035")]
    public async Task Refresh_rotates_and_old_refresh_is_rejected()
    {
        var login = await _client.PostAsJsonAsync("/api/identity/token",
            new DevLoginRequest("agent@crm.local", "Crm!123"));
        var first = await login.Content.ReadFromJsonAsync<TokenResponseDto>();

        var refresh = await _client.PostAsJsonAsync("/api/identity/token/refresh",
            new RefreshTokenRequest(first!.RefreshToken));
        refresh.StatusCode.Should().Be(HttpStatusCode.OK);
        var second = await refresh.Content.ReadFromJsonAsync<TokenResponseDto>();
        second!.RefreshToken.Should().NotBe(first.RefreshToken);

        var reuse = await _client.PostAsJsonAsync("/api/identity/token/refresh",
            new RefreshTokenRequest(first.RefreshToken));
        reuse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait("Story", "CRM-035")]
    public async Task Revoke_blocks_refresh()
    {
        var login = await _client.PostAsJsonAsync("/api/identity/token",
            new DevLoginRequest("lead@crm.local", "Crm!123"));
        var tokens = await login.Content.ReadFromJsonAsync<TokenResponseDto>();

        var revoke = await _client.PostAsJsonAsync("/api/identity/token/revoke",
            new RevokeTokenRequest(tokens!.RefreshToken, tokens.AccessToken));
        revoke.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var refresh = await _client.PostAsJsonAsync("/api/identity/token/refresh",
            new RefreshTokenRequest(tokens.RefreshToken));
        refresh.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait("Story", "CRM-035")]
    public async Task Max_failed_logins_locks_account()
    {
        var email = $"lock-{Guid.NewGuid():N}@crm.local";
        _client.DefaultRequestHeaders.Remove("X-Crm-User-Role");
        _client.DefaultRequestHeaders.Add("X-Crm-User-Role", "Admin");
        var create = await _client.PostAsJsonAsync("/api/identity/users",
            new CreateUserRequest(email, "Lock Me", "Crm!123", "Agent"));
        create.EnsureSuccessStatusCode();

        for (var i = 0; i < UserAccountMaxAttempts(); i++)
        {
            var bad = await _client.PostAsJsonAsync("/api/identity/token",
                new DevLoginRequest(email, "wrong-password"));
            bad.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Locked);
        }

        var locked = await _client.PostAsJsonAsync("/api/identity/token",
            new DevLoginRequest(email, "Crm!123"));
        locked.StatusCode.Should().Be(HttpStatusCode.Locked);
    }

    [Fact]
    [Trait("Story", "CRM-035")]
    public async Task Admin_can_create_assign_and_deactivate_user()
    {
        _client.DefaultRequestHeaders.Remove("X-Crm-User-Role");
        _client.DefaultRequestHeaders.Remove("X-Crm-User-Id");
        _client.DefaultRequestHeaders.Add("X-Crm-User-Role", "Admin");
        _client.DefaultRequestHeaders.Add("X-Crm-User-Id", "33333333-3333-3333-3333-333333333333");

        var email = $"agent-{Guid.NewGuid():N}@crm.local";
        var create = await _client.PostAsJsonAsync("/api/identity/users",
            new CreateUserRequest(email, "Temp Agent", "Crm!123", "Agent"));
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await create.Content.ReadFromJsonAsync<UserSummaryDto>();
        created.Should().NotBeNull();

        var role = await _client.PostAsJsonAsync($"/api/identity/users/{created!.Id}/role",
            new UpdateUserRoleRequest("Lead"));
        role.StatusCode.Should().Be(HttpStatusCode.OK);
        (await role.Content.ReadFromJsonAsync<UserSummaryDto>())!.Role.Should().Be("Lead");

        var deactivate = await _client.PostAsync($"/api/identity/users/{created.Id}/deactivate", null);
        deactivate.StatusCode.Should().Be(HttpStatusCode.OK);

        var login = await _client.PostAsJsonAsync("/api/identity/token",
            new DevLoginRequest(email, "Crm!123"));
        login.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait("Story", "CRM-035")]
    public async Task Non_admin_cannot_create_user()
    {
        _client.DefaultRequestHeaders.Remove("X-Crm-User-Role");
        _client.DefaultRequestHeaders.Add("X-Crm-User-Role", "Agent");
        var response = await _client.PostAsJsonAsync("/api/identity/users",
            new CreateUserRequest("x@crm.local", "X", "Crm!123", "Agent"));
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Story", "CRM-035")]
    public async Task Roles_list_includes_permissions()
    {
        var roles = await _client.GetFromJsonAsync<List<RoleSummaryDto>>("/api/identity/roles");
        roles.Should().Contain(r => r.Name == "Admin" && r.Permissions.Contains("users.manage"));
        roles.Should().Contain(r => r.Name == "Agent");
    }

    [Fact]
    [Trait("Story", "CRM-036")]
    public async Task Admin_can_list_audit_and_see_login_and_user_events()
    {
        _client.DefaultRequestHeaders.Remove("X-Crm-User-Role");
        _client.DefaultRequestHeaders.Remove("X-Crm-User-Id");
        _client.DefaultRequestHeaders.Add("X-Crm-User-Role", "Admin");
        _client.DefaultRequestHeaders.Add("X-Crm-User-Id", "33333333-3333-3333-3333-333333333333");

        var email = $"audit-{Guid.NewGuid():N}@crm.local";
        var create = await _client.PostAsJsonAsync("/api/identity/users",
            new CreateUserRequest(email, "Audit Target", "Crm!123", "Agent"));
        create.EnsureSuccessStatusCode();

        var login = await _client.PostAsJsonAsync("/api/identity/token",
            new DevLoginRequest(email, "Crm!123"));
        login.EnsureSuccessStatusCode();

        var audit = await _client.GetFromJsonAsync<List<AuditLogDto>>("/api/identity/audit");
        audit.Should().NotBeNull();
        audit!.Should().Contain(e => e.Action == "UserCreated" && e.TargetEmail == email && e.Success);
        audit.Should().Contain(e => e.Action == "Login" && e.ActorEmail == email && e.Success);

        var filtered = await _client.GetFromJsonAsync<List<AuditLogDto>>($"/api/identity/audit?q={Uri.EscapeDataString(email)}");
        filtered!.Should().OnlyContain(e =>
            (e.ActorEmail != null && e.ActorEmail.Contains(email, StringComparison.OrdinalIgnoreCase)) ||
            (e.TargetEmail != null && e.TargetEmail.Contains(email, StringComparison.OrdinalIgnoreCase)) ||
            (e.Detail != null && e.Detail.Contains(email, StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    [Trait("Story", "CRM-036")]
    public async Task Non_admin_cannot_list_audit()
    {
        _client.DefaultRequestHeaders.Remove("X-Crm-User-Role");
        _client.DefaultRequestHeaders.Add("X-Crm-User-Role", "Agent");
        var response = await _client.GetAsync("/api/identity/audit");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Story", "CRM-037")]
    public async Task Admin_can_get_and_update_system_settings()
    {
        AsAdmin();

        var current = await _client.GetFromJsonAsync<SystemSettingsDto>("/api/identity/settings");
        current.Should().NotBeNull();
        current!.OrganizationName.Should().NotBeNullOrWhiteSpace();
        current.MaxFailedLoginAttempts.Should().BeGreaterThan(0);

        var update = await _client.PutAsJsonAsync("/api/identity/settings",
            new UpdateSystemSettingsRequest(
                "CRM Demo Org",
                "help@crm.local",
                "ar",
                current.MaxFailedLoginAttempts,
                current.LockoutMinutes));
        update.StatusCode.Should().Be(HttpStatusCode.OK);
        var saved = await update.Content.ReadFromJsonAsync<SystemSettingsDto>();
        saved!.OrganizationName.Should().Be("CRM Demo Org");
        saved.SupportEmail.Should().Be("help@crm.local");
        saved.DefaultCulture.Should().Be("ar");

        var audit = await _client.GetFromJsonAsync<List<AuditLogDto>>("/api/identity/audit?q=SettingsUpdated");
        audit!.Should().Contain(e => e.Action == "SettingsUpdated" && e.Success);

        // restore defaults for sibling tests sharing the fixture DB
        await _client.PutAsJsonAsync("/api/identity/settings",
            new UpdateSystemSettingsRequest(
                "Customer Support CRM",
                "support@crm.local",
                "en",
                5,
                15));
    }

    [Fact]
    [Trait("Story", "CRM-037")]
    public async Task Settings_lockout_policy_is_applied_on_failed_login()
    {
        AsAdmin();
        await _client.PutAsJsonAsync("/api/identity/settings",
            new UpdateSystemSettingsRequest(
                "Customer Support CRM",
                "support@crm.local",
                "en",
                2,
                15));

        var email = $"lock2-{Guid.NewGuid():N}@crm.local";
        (await _client.PostAsJsonAsync("/api/identity/users",
            new CreateUserRequest(email, "Lock Two", "Crm!123", "Agent"))).EnsureSuccessStatusCode();

        for (var i = 0; i < 2; i++)
        {
            var bad = await _client.PostAsJsonAsync("/api/identity/token",
                new DevLoginRequest(email, "wrong-password"));
            bad.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.Locked);
        }

        var locked = await _client.PostAsJsonAsync("/api/identity/token",
            new DevLoginRequest(email, "Crm!123"));
        locked.StatusCode.Should().Be(HttpStatusCode.Locked);

        await _client.PutAsJsonAsync("/api/identity/settings",
            new UpdateSystemSettingsRequest(
                "Customer Support CRM",
                "support@crm.local",
                "en",
                5,
                15));
    }

    [Fact]
    [Trait("Story", "CRM-037")]
    public async Task Non_admin_cannot_read_or_update_settings()
    {
        _client.DefaultRequestHeaders.Remove("X-Crm-User-Role");
        _client.DefaultRequestHeaders.Add("X-Crm-User-Role", "Agent");
        (await _client.GetAsync("/api/identity/settings")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await _client.PutAsJsonAsync("/api/identity/settings",
            new UpdateSystemSettingsRequest("X", "a@b.c", "en", 5, 15)))
            .StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Story", "CRM-037")]
    public async Task Update_settings_rejects_invalid_payload()
    {
        AsAdmin();
        var response = await _client.PutAsJsonAsync("/api/identity/settings",
            new UpdateSystemSettingsRequest("", "not-an-email", "fr", 0, 0));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private void AsAdmin()
    {
        _client.DefaultRequestHeaders.Remove("X-Crm-User-Role");
        _client.DefaultRequestHeaders.Remove("X-Crm-User-Id");
        _client.DefaultRequestHeaders.Add("X-Crm-User-Role", "Admin");
        _client.DefaultRequestHeaders.Add("X-Crm-User-Id", "33333333-3333-3333-3333-333333333333");
    }

    [Fact]
    [Trait("Story", "CRM-043")]
    public async Task Admin_can_create_and_list_departments()
    {
        AsAdmin();
        var name = $"Dept-{Guid.NewGuid():N}"[..16];
        var create = await _client.PostAsJsonAsync("/api/identity/departments",
            new CreateDepartmentRequest(name));
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var dept = await create.Content.ReadFromJsonAsync<DepartmentDto>();
        dept.Should().NotBeNull();
        dept!.Name.Should().Be(name);

        var list = await _client.GetAsync("/api/identity/departments");
        list.StatusCode.Should().Be(HttpStatusCode.OK);
        var rows = await list.Content.ReadFromJsonAsync<List<DepartmentDto>>();
        rows!.Should().Contain(d => d.Name == name);
    }

    private static int UserAccountMaxAttempts() => 5;
}

public sealed class IdentityApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dataPath = Path.Combine(Path.GetTempPath(), "crm-identity-tests", Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_dataPath);
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var useSqlServer = string.Equals(
                Environment.GetEnvironmentVariable("CRM_IDENTITY_PROVIDER"),
                "SqlServer",
                StringComparison.OrdinalIgnoreCase);

            var settings = new Dictionary<string, string?>
            {
                ["Identity:Jwt:SigningKey"] = "CrmTestSigningKey-AtLeast-32-Characters-Long!"
            };

            if (useSqlServer)
            {
                var baseCs = Environment.GetEnvironmentVariable("ConnectionStrings__Identity")
                    ?? throw new InvalidOperationException(
                        "CRM_IDENTITY_PROVIDER=SqlServer requires ConnectionStrings__Identity.");
                var sqlBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(baseCs)
                {
                    InitialCatalog = $"CrmIdTest_{Guid.NewGuid():N}"
                };
                settings["Identity:Provider"] = "SqlServer";
                settings["ConnectionStrings:Identity"] = sqlBuilder.ConnectionString;
            }
            else
            {
                settings["Identity:Provider"] = "Sqlite";
                settings["ConnectionStrings:Identity"] = string.Empty;
                settings["Identity:DataPath"] = _dataPath;
            }

            config.AddInMemoryCollection(settings);
        });
    }
}
