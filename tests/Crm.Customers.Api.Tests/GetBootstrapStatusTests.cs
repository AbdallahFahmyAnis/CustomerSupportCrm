using System.Net;
using System.Net.Http.Json;
using Crm.Contracts.Customers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Crm.Customers.Api.Tests;

public sealed class GetBootstrapStatusTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GetBootstrapStatusTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { });
    }

    [Fact]
    [Trait("Story", "CRM-041")]
    public async Task Bootstrap_query_returns_vertical_slice_status()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/customers/bootstrap");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<BootstrapStatusDto>();
        body.Should().NotBeNull();
        body!.Service.Should().Be("customers");
        body.Status.Should().Be("ready");
        body.Slice.Should().Be("001-platform-foundation");
        body.Pattern.Should().Be("vertical-slice-cqrs");
    }
}
