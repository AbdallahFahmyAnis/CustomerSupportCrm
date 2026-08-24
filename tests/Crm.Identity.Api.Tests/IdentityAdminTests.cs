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
    public async Task Login_succeeds_for_seeded_agent_and_admin()
    {
        var agent = await _client.PostAsJsonAsync("/api/identity/dev-login",
            new DevLoginRequest("agent@crm.local", "Crm!123"));
        agent.StatusCode.Should().Be(HttpStatusCode.OK);
        var agentUser = await agent.Content.ReadFromJsonAsync<DevUserDto>();
        agentUser!.Role.Should().Be("Agent");

        var admin = await _client.PostAsJsonAsync("/api/identity/dev-login",
            new DevLoginRequest("admin@crm.local", "Crm!123"));
        admin.StatusCode.Should().Be(HttpStatusCode.OK);
        (await admin.Content.ReadFromJsonAsync<DevUserDto>())!.Role.Should().Be("Admin");
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

        var login = await _client.PostAsJsonAsync("/api/identity/dev-login",
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
}

public sealed class IdentityApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dataPath = Path.Combine(Path.GetTempPath(), "crm-identity-tests", Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_dataPath);
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Identity:DataPath"] = _dataPath
            });
        });
    }
}
