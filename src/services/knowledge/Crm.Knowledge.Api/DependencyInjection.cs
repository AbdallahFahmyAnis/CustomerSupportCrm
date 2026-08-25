using Crm.BuildingBlocks.Diagnostics;
using Crm.BuildingBlocks.Endpoints;
using Crm.Knowledge.Api.Infrastructure;
using Crm.Knowledge.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Crm.Knowledge.Api;

/// <summary>SDD CRM-021 — DI for Knowledge API.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddKnowledgeApi(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddEndpoints(typeof(DependencyInjection).Assembly);

        services.AddDbContextFactory<KnowledgeDbContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            ConfigureDb(options, config, env);
        }, ServiceLifetime.Singleton);

        services.AddSingleton<KnowledgeDb>();
        return services;
    }

    private static void ConfigureDb(
        DbContextOptionsBuilder options,
        IConfiguration config,
        IWebHostEnvironment env)
    {
        var provider = (config["CRM_KNOWLEDGE_PROVIDER"] ?? config["Knowledge:Provider"] ?? "").Trim();
        var sqlCs = config.GetConnectionString("Knowledge");
        var useSqlServer = provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
                           || (!string.IsNullOrWhiteSpace(sqlCs)
                               && !provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase));

        if (useSqlServer)
        {
            if (string.IsNullOrWhiteSpace(sqlCs))
            {
                throw new InvalidOperationException(
                    "Knowledge Provider=SqlServer requires ConnectionStrings:Knowledge.");
            }

            EnsureSqlServerDatabase(sqlCs);
            options.UseSqlServer(sqlCs);
            return;
        }

        var dataRoot = Path.GetFullPath(
            config["Knowledge:DataPath"] ?? Path.Combine(env.ContentRootPath, "data"));
        Directory.CreateDirectory(dataRoot);
        options.UseSqlite($"Data Source={Path.Combine(dataRoot, "knowledge-ef.db")};Cache=Shared");
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
            throw new InvalidOperationException("Knowledge SQL catalog name is invalid.");
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

    public static WebApplication UseKnowledgeApi(this WebApplication app)
    {
        app.UseCorrelationId();
        var db = app.Services.GetRequiredService<KnowledgeDb>();
        db.EnsureSchema();
        db.SeedIfEmpty();
        app.MapEndpoints();
        return app;
    }
}
