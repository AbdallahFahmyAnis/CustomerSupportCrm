using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Crm.BuildingBlocks.Endpoints;

public static class EndpointMappingExtensions
{
    /// <summary>Register concrete <see cref="IEndpoint"/> types from an assembly for later mapping.</summary>
    public static IServiceCollection AddEndpoints(this IServiceCollection services, params System.Reflection.Assembly[] assemblies)
    {
        var endpointTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IEndpoint).IsAssignableFrom(t));

        foreach (var type in endpointTypes)
        {
            services.AddSingleton(typeof(IEndpoint), type);
        }

        return services;
    }

    /// <summary>Map every registered <see cref="IEndpoint"/> onto the app.</summary>
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        foreach (var endpoint in app.Services.GetServices<IEndpoint>())
        {
            endpoint.MapEndpoint(app);
        }

        return app;
    }
}
