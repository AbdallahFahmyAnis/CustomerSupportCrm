using Microsoft.AspNetCore.Builder;

namespace Crm.BuildingBlocks.Diagnostics;

public static class CorrelationIdExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        => app.UseMiddleware<CorrelationIdMiddleware>();
}
