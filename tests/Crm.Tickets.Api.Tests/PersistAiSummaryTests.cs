using System.Net;
using System.Net.Http.Json;
using Crm.Contracts.Tickets;
using FluentAssertions;
using Xunit;

namespace Crm.Tickets.Api.Tests;

/// <summary>SDD CRM-023 polish / specs/042</summary>
public sealed class PersistAiSummaryTests : IClassFixture<TicketsApiFactory>
{
    private readonly HttpClient _client;

    public PersistAiSummaryTests(TicketsApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    [Trait("Story", "CRM-023")]
    public async Task Put_ai_summary_persists_and_returns_on_detail()
    {
        var create = await _client.PostAsJsonAsync("/api/tickets", new CreateTicketRequest(
            Guid.NewGuid().ToString(),
            "Summary Co",
            "Need persisted AI summary",
            "Description for summary",
            "Billing",
            "High"));
        create.EnsureSuccessStatusCode();
        var created = (await create.Content.ReadFromJsonAsync<TicketSummaryDto>())!;

        var put = await _client.PutAsJsonAsync(
            $"/api/tickets/{created.Id}/ai-summary",
            new UpdateAiSummaryRequest("Heuristic summary of the ticket.", ["Billing", "High"]));
        put.EnsureSuccessStatusCode();
        var detail = await put.Content.ReadFromJsonAsync<TicketDetailDto>();
        detail!.AiSummary.Should().Be("Heuristic summary of the ticket.");
        detail.AiHighlights.Should().Contain("Billing");
        detail.AiSummaryAt.Should().NotBeNull();

        var get = await _client.GetFromJsonAsync<TicketDetailDto>($"/api/tickets/{created.Id}");
        get!.AiSummary.Should().Be("Heuristic summary of the ticket.");
        get.AiHighlights.Should().BeEquivalentTo(["Billing", "High"]);

        var bad = await _client.PutAsJsonAsync(
            $"/api/tickets/{created.Id}/ai-summary",
            new UpdateAiSummaryRequest("  "));
        bad.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
