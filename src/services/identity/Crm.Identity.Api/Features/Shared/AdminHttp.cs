using Crm.Identity.Api.Domain;

namespace Crm.Identity.Api.Features.Shared;

internal static class AdminHttp
{
    public static bool IsAdmin(HttpContext http) =>
        string.Equals(
            http.Request.Headers["X-Crm-User-Role"].FirstOrDefault(),
            RoleNames.Admin,
            StringComparison.OrdinalIgnoreCase);

    public static Guid? ActorId(HttpContext http)
    {
        var raw = http.Request.Headers["X-Crm-User-Id"].FirstOrDefault();
        return Guid.TryParse(raw, out var id) ? id : null;
    }

    public static IResult ForbiddenAdmin() =>
        Results.Json(new { error = "Admin role required." }, statusCode: StatusCodes.Status403Forbidden);
}
