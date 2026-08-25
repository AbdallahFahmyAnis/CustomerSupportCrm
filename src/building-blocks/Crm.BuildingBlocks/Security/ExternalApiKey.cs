namespace Crm.BuildingBlocks.Security;

/// <summary>SDD CRM-038 — static API key check for /api/external/v1.</summary>
public static class ExternalApiKey
{
    public static bool IsAuthorized(string? configuredKey, string? xApiKeyHeader, string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(configuredKey))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(xApiKeyHeader) &&
            string.Equals(xApiKeyHeader.Trim(), configuredKey, StringComparison.Ordinal))
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            return false;
        }

        const string prefix = "ApiKey ";
        if (!authorizationHeader.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var presented = authorizationHeader[prefix.Length..].Trim();
        return string.Equals(presented, configuredKey, StringComparison.Ordinal);
    }
}
