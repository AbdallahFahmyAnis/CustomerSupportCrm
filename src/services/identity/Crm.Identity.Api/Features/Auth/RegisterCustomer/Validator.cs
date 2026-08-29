using Crm.Identity.Api.Domain;

namespace Crm.Identity.Api.Features.Auth.RegisterCustomer;

/// <summary>SDD CRM-045.</summary>
internal static class RegisterCustomerValidator
{
    public static string? Validate(RegisterCustomerCommand request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return "Email is required.";
        }

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            return "Display name is required.";
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return "Password is required.";
        }

        if (request.Password.Length < 6)
        {
            return "Password must be at least 6 characters.";
        }

        return null;
    }
}
