using Crm.Identity.Api.Domain;

namespace Crm.Identity.Api.Features.Users.UpdateUserRole;

public static class UpdateUserRoleValidator
{
    public static string? Validate(UpdateUserRoleCommand command)
    {
        if (command.UserId == Guid.Empty)
        {
            return "User id is required.";
        }

        if (string.IsNullOrWhiteSpace(command.Role) ||
            !RoleNames.All.Contains(command.Role.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            return "Role must be one of: " + string.Join(", ", RoleNames.All);
        }

        return null;
    }
}
