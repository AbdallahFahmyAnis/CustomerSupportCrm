using System.Net;
using System.Net.Http.Json;
using Crm.Contracts.Sla;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Crm.Sla.Api.Tests;

public sealed class SlaTargetsTests : IClassFixture<SlaApiFactory>
{
    private readonly HttpClient _client;

    public SlaTargetsTests(SlaApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    [Trait("Story", "CRM-017")]
    public async Task Seed_lists_four_priority_policies()
    {
        var policies = await _client.GetFromJsonAsync<List<SlaPolicyDto>>("/api/sla/policies");
        policies.Should().NotBeNull();
        policies!.Select(p => p.Priority).Should().BeEquivalentTo(["Low", "Medium", "High", "Urgent"]);
        policies.Should().OnlyContain(p => p.FirstResponseMinutes > 0 && p.ResolutionMinutes > 0);
    }

    [Fact]
    [Trait("Story", "CRM-017")]
    public async Task Update_high_policy_and_reject_invalid_minutes()
    {
        var ok = await _client.PutAsJsonAsync("/api/sla/policies/High",
            new UpdateSlaPolicyRequest(45, 400));
        ok.EnsureSuccessStatusCode();
        var updated = await ok.Content.ReadFromJsonAsync<SlaPolicyDto>();
        updated!.FirstResponseMinutes.Should().Be(45);
        updated.ResolutionMinutes.Should().Be(400);

        var bad = await _client.PutAsJsonAsync("/api/sla/policies/High",
            new UpdateSlaPolicyRequest(0, 100));
        bad.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var unknown = await _client.PutAsJsonAsync("/api/sla/policies/Critical",
            new UpdateSlaPolicyRequest(10, 20));
        unknown.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Story", "CRM-017")]
    public async Task Evaluate_computes_dues_and_breach()
    {
        var created = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var asOfOk = created.AddMinutes(10);
        var ok = await _client.PostAsJsonAsync("/api/sla/evaluate", new EvaluateSlaRequest(
            "Urgent",
            created,
            AsOf: asOfOk));
        ok.EnsureSuccessStatusCode();
        var evaluation = await ok.Content.ReadFromJsonAsync<SlaEvaluationDto>();
        evaluation!.FirstResponseDueAt.Should().Be(created.AddMinutes(15));
        evaluation.ResolutionDueAt.Should().Be(created.AddMinutes(240));
        evaluation.FirstResponseBreached.Should().BeFalse();
        evaluation.ResolutionBreached.Should().BeFalse();

        var asOfBreach = created.AddMinutes(20);
        var breached = await _client.PostAsJsonAsync("/api/sla/evaluate", new EvaluateSlaRequest(
            "Urgent",
            created,
            AsOf: asOfBreach));
        breached.EnsureSuccessStatusCode();
        var late = await breached.Content.ReadFromJsonAsync<SlaEvaluationDto>();
        late!.FirstResponseBreached.Should().BeTrue();
        late.ResolutionBreached.Should().BeFalse();
    }

    [Fact]
    [Trait("Story", "CRM-018")]
    public async Task Suggest_assignee_prefers_priority_over_default()
    {
        var urgent = await _client.PostAsJsonAsync("/api/sla/suggest-assignee",
            new SuggestAssigneeRequest("Billing", "Urgent"));
        urgent.EnsureSuccessStatusCode();
        var lead = await urgent.Content.ReadFromJsonAsync<SuggestAssigneeDto>();
        lead!.AgentName.Should().Be("Lead Agent");

        var technical = await _client.PostAsJsonAsync("/api/sla/suggest-assignee",
            new SuggestAssigneeRequest("Technical", "Low"));
        technical.EnsureSuccessStatusCode();
        var demo = await technical.Content.ReadFromJsonAsync<SuggestAssigneeDto>();
        demo!.AgentName.Should().Be("Demo Agent");
    }

    [Fact]
    [Trait("Story", "CRM-019")]
    public async Task Should_escalate_urgent_and_skip_when_already_escalated()
    {
        var created = DateTimeOffset.UtcNow;
        var yes = await _client.PostAsJsonAsync("/api/sla/should-escalate",
            new ShouldEscalateRequest("Urgent", created, IsEscalated: false, Status: "New"));
        yes.EnsureSuccessStatusCode();
        var decision = await yes.Content.ReadFromJsonAsync<ShouldEscalateDto>();
        decision!.ShouldEscalate.Should().BeTrue();
        decision.AssignToAgentName.Should().Be("Lead Agent");

        var no = await _client.PostAsJsonAsync("/api/sla/should-escalate",
            new ShouldEscalateRequest("Urgent", created, IsEscalated: true, Status: "New"));
        no.EnsureSuccessStatusCode();
        var skipped = await no.Content.ReadFromJsonAsync<ShouldEscalateDto>();
        skipped!.ShouldEscalate.Should().BeFalse();
    }
}

public sealed class SlaApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dataPath = Path.Combine(Path.GetTempPath(), "crm-sla-tests", Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_dataPath);
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Sla:DataPath"] = _dataPath,
                ["Sla:Provider"] = "Sqlite",
                ["ConnectionStrings:Sla"] = string.Empty
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
