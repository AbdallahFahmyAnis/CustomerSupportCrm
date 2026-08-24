using Crm.BuildingBlocks.Diagnostics;
using Crm.BuildingBlocks.Endpoints;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApi(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddEndpoints(typeof(DependencyInjection).Assembly);
        services.AddSingleton<IdentityDb>();
        services.AddSingleton<TokenService>();
        return services;
    }

    public static WebApplication UseIdentityApi(this WebApplication app)
    {
        app.UseCorrelationId();

        var db = app.Services.GetRequiredService<IdentityDb>();
        db.EnsureSchema();
        db.SeedIfEmpty();

        app.MapEndpoints();
        return app;
    }
}
