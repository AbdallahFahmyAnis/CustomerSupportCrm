using Crm.BuildingBlocks.Security;
using FluentAssertions;
using Xunit;

namespace Crm.Gateway.Tests;

/// <summary>SDD CRM-038 polish / specs/041 + deferred / 045</summary>
public sealed class ExternalApiOpenApiTests
{
    [Fact]
    [Trait("Story", "CRM-038")]
    public void OpenApi_yaml_documents_tickets_and_customers()
    {
        ExternalApiOpenApi.Yaml.Should().Contain("/api/external/v1/tickets");
        ExternalApiOpenApi.Yaml.Should().Contain("/api/external/v1/customers");
        ExternalApiOpenApi.Yaml.Should().Contain("X-Api-Key");
        ExternalApiOpenApi.Yaml.Should().Contain("openapi: 3.0.3");
    }

    [Fact]
    [Trait("Story", "CRM-038")]
    public void Swagger_ui_html_loads_openapi_yaml()
    {
        ExternalApiSwaggerUi.Html.Should().Contain("swagger-ui");
        ExternalApiSwaggerUi.Html.Should().Contain("/api/external/v1/openapi.yaml");
        ExternalApiSwaggerUi.Html.Should().Contain("SwaggerUIBundle");
    }
}
