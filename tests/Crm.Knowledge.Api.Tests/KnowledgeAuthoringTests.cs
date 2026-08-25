using System.Net;
using System.Net.Http.Json;
using Crm.Contracts.Knowledge;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Crm.Knowledge.Api.Tests;

public sealed class KnowledgeAuthoringTests : IClassFixture<KnowledgeApiFactory>
{
    private readonly HttpClient _client;

    public KnowledgeAuthoringTests(KnowledgeApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    [Trait("Story", "CRM-021")]
    public async Task Seed_lists_articles_and_filter_works()
    {
        var all = await _client.GetFromJsonAsync<List<KnowledgeArticleSummaryDto>>("/api/knowledge/articles");
        all.Should().NotBeNull();
        all!.Count.Should().BeGreaterThanOrEqualTo(2);
        all.Should().Contain(a => a.Kind == "Faq");
        all.Should().Contain(a => a.Kind == "Solution");

        var filtered = await _client.GetFromJsonAsync<List<KnowledgeArticleSummaryDto>>(
            "/api/knowledge/articles?q=password");
        filtered!.Should().Contain(a => a.Title.Contains("password", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    [Trait("Story", "CRM-022")]
    public async Task Ranked_search_requires_query_and_ranks_title_hits()
    {
        var bad = await _client.GetAsync("/api/knowledge/search");
        bad.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var hits = await _client.GetFromJsonAsync<List<KnowledgeSearchHitDto>>(
            "/api/knowledge/search?q=password&publishedOnly=true");
        hits.Should().NotBeNull();
        hits!.Should().NotBeEmpty();
        hits[0].Title.Should().ContainEquivalentOf("password");
        hits[0].Score.Should().BeGreaterThan(0);
        hits[0].Snippet.Should().NotBeNullOrWhiteSpace();

        var solutions = await _client.GetFromJsonAsync<List<KnowledgeSearchHitDto>>(
            "/api/knowledge/search?q=invoice&kind=Solution");
        solutions!.Should().OnlyContain(h => h.Kind == "Solution");
    }

    [Fact]
    [Trait("Story", "CRM-021")]
    public async Task Create_update_and_reject_invalid()
    {
        var bad = await _client.PostAsJsonAsync("/api/knowledge/articles",
            new CreateKnowledgeArticleRequest("", "body", "Faq", "Draft"));
        bad.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var created = await _client.PostAsJsonAsync("/api/knowledge/articles",
            new CreateKnowledgeArticleRequest("WhatsApp token guide", "Rotate the token in admin.", "Guide", "Draft"));
        created.EnsureSuccessStatusCode();
        var detail = await created.Content.ReadFromJsonAsync<KnowledgeArticleDetailDto>();
        detail!.Kind.Should().Be("Guide");
        detail.Status.Should().Be("Draft");

        var updated = await _client.PutAsJsonAsync($"/api/knowledge/articles/{detail.Id}",
            new UpdateKnowledgeArticleRequest(detail.Title, "Updated body.", "Guide", "Published"));
        updated.EnsureSuccessStatusCode();
        var saved = await updated.Content.ReadFromJsonAsync<KnowledgeArticleDetailDto>();
        saved!.Status.Should().Be("Published");
        saved.Body.Should().Be("Updated body.");
    }
}

public sealed class KnowledgeApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dataPath = Path.Combine(Path.GetTempPath(), "crm-knowledge-tests", Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_dataPath);
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Knowledge:DataPath"] = _dataPath,
                ["Knowledge:Provider"] = "Sqlite",
                ["ConnectionStrings:Knowledge"] = string.Empty
            });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        try
        {
            if (Directory.Exists(_dataPath))
            {
                Directory.Delete(_dataPath, true);
            }
        }
        catch
        {
        }
    }
}
