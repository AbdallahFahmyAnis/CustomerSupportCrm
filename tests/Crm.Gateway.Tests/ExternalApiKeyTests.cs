using Crm.BuildingBlocks.Security;
using FluentAssertions;
using Xunit;

namespace Crm.Gateway.Tests;

/// <summary>SDD CRM-038</summary>
public sealed class ExternalApiKeyTests
{
    [Fact]
    [Trait("Story", "CRM-038")]
    public void Accepts_X_Api_Key_header()
    {
        ExternalApiKey.IsAuthorized("dev-external-key", "dev-external-key", null).Should().BeTrue();
    }

    [Fact]
    [Trait("Story", "CRM-038")]
    public void Accepts_Authorization_ApiKey_scheme()
    {
        ExternalApiKey.IsAuthorized("dev-external-key", null, "ApiKey dev-external-key").Should().BeTrue();
    }

    [Fact]
    [Trait("Story", "CRM-038")]
    public void Rejects_missing_or_wrong_key()
    {
        ExternalApiKey.IsAuthorized("dev-external-key", null, null).Should().BeFalse();
        ExternalApiKey.IsAuthorized("dev-external-key", "nope", null).Should().BeFalse();
        ExternalApiKey.IsAuthorized("", "dev-external-key", null).Should().BeFalse();
    }
}
