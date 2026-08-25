namespace Crm.Identity.Api.Features.Settings.UpdateSettings;

public static class UpdateSettingsValidator
{
    private static readonly HashSet<string> Cultures = new(StringComparer.OrdinalIgnoreCase) { "en", "ar" };

    public static string? Validate(UpdateSettingsCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.OrganizationName))
        {
            return "Organization name is required.";
        }

        if (string.IsNullOrWhiteSpace(command.SupportEmail) || !command.SupportEmail.Contains('@'))
        {
            return "A valid support email is required.";
        }

        if (string.IsNullOrWhiteSpace(command.DefaultCulture) || !Cultures.Contains(command.DefaultCulture.Trim()))
        {
            return "Default culture must be en or ar.";
        }

        if (command.MaxFailedLoginAttempts is < 1 or > 20)
        {
            return "Max failed login attempts must be between 1 and 20.";
        }

        if (command.LockoutMinutes is < 1 or > 1440)
        {
            return "Lockout minutes must be between 1 and 1440.";
        }

        return null;
    }
}
