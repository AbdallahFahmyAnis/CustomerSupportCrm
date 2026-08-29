using Microsoft.AspNetCore.Http;

namespace Crm.BuildingBlocks.Audit;

/// <summary>SDD CRM-036 / specs/051 — read actor from gateway identity headers.</summary>
public static class AuditActor
{
    public static string? Email(IHttpContextAccessor accessor)
    {
        var http = accessor.HttpContext;
        if (http is null)
        {
            return null;
        }

        return http.Request.Headers["X-Crm-User-Email"].FirstOrDefault()
               ?? http.Request.Headers["X-Crm-User-Id"].FirstOrDefault();
    }
}
