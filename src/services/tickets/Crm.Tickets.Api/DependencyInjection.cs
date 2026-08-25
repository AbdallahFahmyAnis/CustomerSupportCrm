using Crm.BuildingBlocks.Diagnostics;
using Crm.BuildingBlocks.Endpoints;
using Crm.Tickets.Api.Infrastructure;
using Crm.Tickets.Api.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Crm.Tickets.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddTicketsApi(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddEndpoints(typeof(DependencyInjection).Assembly);

        services.AddDbContextFactory<TicketsDbContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            ConfigureDb(options, config, env);
        }, ServiceLifetime.Singleton);

        services.AddSingleton<TicketsDb>();
        return services;
    }

    private static void ConfigureDb(
        DbContextOptionsBuilder options,
        IConfiguration config,
        IWebHostEnvironment env)
    {
        var provider = (config["CRM_TICKETS_PROVIDER"] ?? config["Tickets:Provider"] ?? "").Trim();
        var sqlCs = config.GetConnectionString("Tickets");
        var useSqlServer = provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)
                           || (!string.IsNullOrWhiteSpace(sqlCs)
                               && !provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase));

        if (useSqlServer)
        {
            if (string.IsNullOrWhiteSpace(sqlCs))
            {
                throw new InvalidOperationException(
                    "Tickets Provider=SqlServer requires ConnectionStrings:Tickets.");
            }

            EnsureSqlServerDatabase(sqlCs);
            options.UseSqlServer(sqlCs);
            return;
        }

        var dataRoot = Path.GetFullPath(
            config["Tickets:DataPath"] ?? Path.Combine(env.ContentRootPath, "data"));
        Directory.CreateDirectory(dataRoot);
        options.UseSqlite($"Data Source={Path.Combine(dataRoot, "tickets-ef.db")};Cache=Shared");
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
            throw new InvalidOperationException("Tickets SQL catalog name is invalid.");
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

    public static WebApplication UseTicketsApi(this WebApplication app)
    {
        app.UseCorrelationId();
        var db = app.Services.GetRequiredService<TicketsDb>();
        db.EnsureSchema();
        db.SeedIfEmpty();
        app.MapEndpoints();
        return app;
    }
}
