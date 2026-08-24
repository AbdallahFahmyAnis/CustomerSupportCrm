using Crm.BuildingBlocks.Diagnostics;
using Crm.BuildingBlocks.Endpoints;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddCustomersApi(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddEndpoints(typeof(DependencyInjection).Assembly);
        services.AddSingleton<CustomersDb>();
        return services;
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
