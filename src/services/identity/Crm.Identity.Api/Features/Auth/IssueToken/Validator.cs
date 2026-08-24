namespace Crm.Identity.Api.Features.Auth.IssueToken;

public static class IssueTokenValidator
{
    public static string? Validate(IssueTokenCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrWhiteSpace(command.Password))
        {
            return "Email and password are required.";
        }

        return null;
    }
}
