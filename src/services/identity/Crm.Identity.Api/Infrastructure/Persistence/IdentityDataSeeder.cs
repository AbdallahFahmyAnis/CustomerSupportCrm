using Crm.BuildingBlocks.Identity;
using Crm.Identity.Api.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Crm.Identity.Api.Infrastructure.Persistence;

/// <summary>SDD CRM-035 / CRM-037 — seed roles, demo users, OpenIddict gateway client.</summary>
public sealed class IdentityDataSeeder(
    RoleManager<IdentityRole<Guid>> roles,
    UserManager<ApplicationUser> users,
    IOpenIddictApplicationManager applications,
    IConfiguration config)
{
    public const string PermissionClaimType = "permission";
    public const string GatewayClientId = "crm-gateway";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureRoleAsync(RoleNames.Admin, "Full administration",
            [
                PermissionCatalog.UsersManage,
                PermissionCatalog.RolesView,
                PermissionCatalog.TicketsAll,
                PermissionCatalog.CustomersAll
            ], cancellationToken);

            await EnsureRoleAsync(RoleNames.Lead, "Team lead",
            [
                PermissionCatalog.TicketsAll,
                PermissionCatalog.TicketsAssign,
                PermissionCatalog.CustomersAll
            ], cancellationToken);

            await EnsureRoleAsync(RoleNames.Agent, "Support agent",
            [
                PermissionCatalog.TicketsWork,
                PermissionCatalog.CustomersRead
            ], cancellationToken);

            await EnsureUserAsync(
                Guid.Parse(DevUsers.AgentId),
                DevUsers.AgentEmail,
                DevUsers.AgentName,
                DevUsers.Password,
                RoleNames.Agent,
                cancellationToken);

            await EnsureUserAsync(
                Guid.Parse("33333333-3333-3333-3333-333333333333"),
                "admin@crm.local",
                "Demo Admin",
                DevUsers.Password,
                RoleNames.Admin,
                cancellationToken);

            await EnsureUserAsync(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "lead@crm.local",
                "Lead Agent",
                DevUsers.Password,
                RoleNames.Lead,
                cancellationToken);

            await EnsureGatewayClientAsync(cancellationToken);
        }
        catch
        {
            // never brick startup
        }
    }

    private async Task EnsureRoleAsync(
        string name,
        string description,
        IEnumerable<string> permissions,
        CancellationToken cancellationToken)
    {
        var role = await roles.FindByNameAsync(name);
        if (role is null)
        {
            role = new IdentityRole<Guid>(name) { Id = Guid.NewGuid() };
            var create = await roles.CreateAsync(role);
            if (!create.Succeeded)
            {
                return;
            }
        }

        // Description stored as claim for admin list UI
        const string descType = "role_description";
        var existingClaims = await roles.GetClaimsAsync(role);
        if (existingClaims.All(c => c.Type != descType))
        {
            await roles.AddClaimAsync(role, new System.Security.Claims.Claim(descType, description));
        }

        foreach (var permission in permissions)
        {
            if (existingClaims.Any(c => c.Type == PermissionClaimType && c.Value == permission))
            {
                continue;
            }

            await roles.AddClaimAsync(role,
                new System.Security.Claims.Claim(PermissionClaimType, permission));
        }
    }

    private async Task EnsureUserAsync(
        Guid id,
        string email,
        string displayName,
        string password,
        string role,
        CancellationToken cancellationToken)
    {
        var user = await users.FindByEmailAsync(email);
        if (user is not null)
        {
            return;
        }

        user = new ApplicationUser
        {
            Id = id,
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            DisplayName = displayName,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = await users.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return;
        }

        await users.AddToRoleAsync(user, role);
    }

    private async Task EnsureGatewayClientAsync(CancellationToken cancellationToken)
    {
        var existing = await applications.FindByClientIdAsync(GatewayClientId, cancellationToken);
        if (existing is not null)
        {
            return;
        }

        var secret = config["Identity:OpenIddict:GatewayClientSecret"] ?? "Crm-Gateway-Local-Secret";
        await applications.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = GatewayClientId,
            ClientSecret = secret,
            DisplayName = "CRM Gateway BFF",
            ClientType = ClientTypes.Confidential,
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Revocation,
                Permissions.GrantTypes.Password,
                Permissions.GrantTypes.RefreshToken,
                Permissions.Prefixes.Scope + "api",
                Permissions.Prefixes.Scope + Scopes.Email,
                Permissions.Prefixes.Scope + Scopes.Profile,
                Permissions.Prefixes.Scope + Scopes.Roles
            }
        }, cancellationToken);
    }
}
