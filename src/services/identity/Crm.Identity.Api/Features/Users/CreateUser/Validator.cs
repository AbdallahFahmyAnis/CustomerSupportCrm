using Crm.Identity.Api.Domain;

namespace Crm.Identity.Api.Features.Users.CreateUser;

public static class CreateUserValidator
{
    public static string? Validate(CreateUserCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return "Email is required.";
        }

        if (string.IsNullOrWhiteSpace(command.DisplayName))
        {
            return "Display name is required.";
        }

        if (string.IsNullOrWhiteSpace(command.Password))
        {
            return "Password is required.";
        }

        if (string.IsNullOrWhiteSpace(command.Role) ||
            !RoleNames.All.Contains(command.Role.Trim(), StringComparer.OrdinalIgnoreCase))
        {
            return "Role must be one of: " + string.Join(", ", RoleNames.All);
        }

        return null;
    }
}
