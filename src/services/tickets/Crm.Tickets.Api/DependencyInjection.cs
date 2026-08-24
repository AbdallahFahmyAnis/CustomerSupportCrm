using Crm.BuildingBlocks.Diagnostics;
using Crm.BuildingBlocks.Endpoints;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddTicketsApi(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddEndpoints(typeof(DependencyInjection).Assembly);
        services.AddSingleton<TicketsDb>();
        return services;
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
