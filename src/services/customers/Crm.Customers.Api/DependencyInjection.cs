using Crm.BuildingBlocks.Diagnostics;
using Crm.BuildingBlocks.Endpoints;
using Crm.Customers.Api.Infrastructure;
using Crm.Customers.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Crm.Customers.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomersApi(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddEndpoints(typeof(DependencyInjection).Assembly);

        services.AddDbContextFactory<CustomersDbContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            ConfigureDb(options, config, env, "Customers");
        }, ServiceLifetime.Singleton);

        services.AddSingleton<CustomersDb>();
        return services;
    }

    private static void ConfigureDb(
        DbContextOptionsBuilder options,
        IConfiguration config,
        IWebHostEnvironment env,
        string section)
    {
        var provider = (config[$"CRM_{section.ToUpperInvariant()}_PROVIDER"]
                        ?? config[$"{section}:Provider"]
                        ?? "").Trim();
        var sqlCs = config.GetConnectionString(section);
        var useSqlServer = provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
                           || (!string.IsNullOrWhiteSpace(sqlCs)
                               && !provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase));

        if (useSqlServer)
        {
            if (string.IsNullOrWhiteSpace(sqlCs))
            {
                throw new InvalidOperationException(
                    $"{section} Provider=SqlServer requires ConnectionStrings:{section}.");
            }

            EnsureSqlServerDatabase(sqlCs);
            options.UseSqlServer(sqlCs);
            return;
        }

        var dataRoot = Path.GetFullPath(
            config[$"{section}:DataPath"] ?? Path.Combine(env.ContentRootPath, "data"));
        Directory.CreateDirectory(dataRoot);
        options.UseSqlite($"Data Source={Path.Combine(dataRoot, $"{section.ToLowerInvariant()}-ef.db")};Cache=Shared");
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
            throw new InvalidOperationException($"{database} catalog name is invalid.");
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

    public static WebApplication UseCustomersApi(this WebApplication app)
    {
        app.UseCorrelationId();
        var db = app.Services.GetRequiredService<CustomersDb>();
        db.EnsureSchema();
        db.SeedIfEmpty();
        app.MapEndpoints();
        return app;
    }
}
