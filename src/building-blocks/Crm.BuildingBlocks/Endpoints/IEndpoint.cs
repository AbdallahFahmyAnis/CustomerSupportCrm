using Microsoft.AspNetCore.Routing;

namespace Crm.BuildingBlocks.Endpoints;

/// <summary>Maps one vertical-slice HTTP endpoint. Discovered by MapEndpoints().</summary>
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
