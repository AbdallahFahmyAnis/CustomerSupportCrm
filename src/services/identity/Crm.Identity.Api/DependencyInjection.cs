using System.Security.Cryptography;
using System.Text;
using Crm.BuildingBlocks.Diagnostics;
using Crm.BuildingBlocks.Endpoints;
using Crm.Identity.Api.Infrastructure;
using Crm.Identity.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Crm.Identity.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApi(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddEndpoints(typeof(DependencyInjection).Assembly);
        services.AddSingleton<TokenService>();
        services.AddScoped<IdentityDirectory>();
        services.AddScoped<IdentityDataSeeder>();

        services.AddDbContext<IdentityAppDbContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            ConfigureDb(options, config, env);
            options.UseOpenIddict();
        });

        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = Domain.UserAccount.MaxFailedAttempts;
                options.Lockout.DefaultLockoutTimeSpan = Domain.UserAccount.LockoutDuration;
            })
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthorization();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
        });

        services.AddOpenIddict()
            .AddCore(options => options.UseEntityFrameworkCore().UseDbContext<IdentityAppDbContext>())
            .AddServer(options =>
            {
                options.SetTokenEndpointUris("/connect/token");

                options.AllowPasswordFlow()
                    .AllowRefreshTokenFlow();

                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, "api");
                options.AcceptAnonymousClients();
                options.DisableAccessTokenEncryption();

                options.AddEphemeralEncryptionKey()
                    .AddEphemeralSigningKey();

                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough();
            })
            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }

    private static void ConfigureDb(
        DbContextOptionsBuilder options,
        IConfiguration config,
        IWebHostEnvironment env)
    {
        var provider = (config["CRM_IDENTITY_PROVIDER"] ?? config["Identity:Provider"] ?? "").Trim();
        var sqlCs = config.GetConnectionString("Identity");
        var useSqlServer = provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
                           || (!string.IsNullOrWhiteSpace(sqlCs)
                               && !provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase));

        if (useSqlServer)
        {
            if (string.IsNullOrWhiteSpace(sqlCs))
            {
                throw new InvalidOperationException(
                    "Identity Provider=SqlServer requires ConnectionStrings:Identity.");
            }

            EnsureSqlServerDatabase(sqlCs);
            options.UseSqlServer(sqlCs);
            return;
        }

        var dataRoot = Path.GetFullPath(
            config["Identity:DataPath"] ?? Path.Combine(env.ContentRootPath, "data"));
        Directory.CreateDirectory(dataRoot);
        options.UseSqlite($"Data Source={Path.Combine(dataRoot, "identity-ef.db")}");
    }

    private static void EnsureSqlServerDatabase(string connectionString)
    {
        var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
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
        using var connection = new Microsoft.Data.SqlClient.SqlConnection(builder.ConnectionString);
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

    public static WebApplication UseIdentityApi(this WebApplication app)
    {
        app.UseCorrelationId();

        using (var scope = app.Services.CreateScope())
        {
            var directory = scope.ServiceProvider.GetRequiredService<IdentityDirectory>();
            directory.EnsureSchemaAsync().GetAwaiter().GetResult();
            var seeder = scope.ServiceProvider.GetRequiredService<IdentityDataSeeder>();
            seeder.SeedAsync().GetAwaiter().GetResult();
        }

        app.MapEndpoints();
        MapPasswordTokenAdapter(app);
        return app;
    }

    /// <summary>
    /// OpenIddict token endpoint passthrough adapter (password + refresh) — CRM-037 Option A.
    /// Gateway BFF continues to use /api/identity/token*; /connect/token is the OIDC-style surface.
    /// </summary>
    private static void MapPasswordTokenAdapter(WebApplication app)
    {
        app.MapPost("/connect/token", async (HttpContext http) =>
        {
            var form = await http.Request.ReadFormAsync();
            var grant = form["grant_type"].ToString();
            var directory = http.RequestServices.GetRequiredService<IdentityDirectory>();
            var now = DateTimeOffset.UtcNow;

            if (grant == "password")
            {
                var email = form["username"].ToString();
                var password = form["password"].ToString();
                var user = await directory.FindByEmailAsync(email);
                if (user is null || !user.IsActive || user.IsLockedOut(now))
                {
                    return Results.BadRequest(new { error = "invalid_grant" });
                }

                if (!await directory.CheckPasswordAsync(user, password))
                {
                    await directory.RegisterFailedLoginAsync(user, now);
                    return Results.BadRequest(new { error = "invalid_grant" });
                }

                await directory.RegisterSuccessfulLoginAsync(user);
                var pair = await directory.IssuePairAsync(user, now);
                return Results.Json(new
                {
                    access_token = pair.AccessToken,
                    refresh_token = pair.RefreshToken,
                    token_type = "Bearer",
                    expires_in = Math.Max(1, (int)(pair.AccessTokenExpiresAt - now).TotalSeconds)
                });
            }

            if (grant == "refresh_token")
            {
                var refresh = form["refresh_token"].ToString();
                var hash = TokenService.HashToken(refresh);
                var existing = await directory.FindRefreshTokenByHashAsync(hash);
                if (existing is null || !existing.IsActive(now))
                {
                    if (existing is not null)
                    {
                        await directory.RevokeAllRefreshTokensForUserAsync(existing.UserId, now);
                    }

                    return Results.BadRequest(new { error = "invalid_grant" });
                }

                var user = await directory.GetUserAsync(existing.UserId);
                if (user is null || !user.IsActive || user.IsLockedOut(now))
                {
                    return Results.BadRequest(new { error = "invalid_grant" });
                }

                var pair = await directory.IssuePairAsync(user, now);
                var replacement = await directory.FindRefreshTokenByHashAsync(
                    TokenService.HashToken(pair.RefreshToken));
                await directory.RevokeRefreshTokenAsync(existing.Id, now, replacement?.Id);
                return Results.Json(new
                {
                    access_token = pair.AccessToken,
                    refresh_token = pair.RefreshToken,
                    token_type = "Bearer",
                    expires_in = Math.Max(1, (int)(pair.AccessTokenExpiresAt - now).TotalSeconds)
                });
            }

            return Results.BadRequest(new { error = "unsupported_grant_type" });
        }).DisableAntiforgery();
    }
}
