using System.Net;
using System.Net.Http.Json;
using Crm.Contracts.Tickets;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Crm.Tickets.Api.Tests;

public sealed class TicketLifecycleTests : IClassFixture<TicketsApiFactory>
{
    private readonly HttpClient _client;

    public TicketLifecycleTests(TicketsApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    [Trait("Story", "CRM-004")]
    public async Task Create_and_search_ticket_by_number_or_customer()
    {
        var created = await CreateAsync("Acme Industries", "Need help with invoice");
        created.TicketNumber.Should().StartWith("TKT-");

        var byNumber = await _client.GetFromJsonAsync<List<TicketSummaryDto>>($"/api/tickets?q={created.TicketNumber}");
        byNumber.Should().Contain(t => t.Id == created.Id);

        var byCustomer = await _client.GetFromJsonAsync<List<TicketSummaryDto>>("/api/tickets?q=Acme");
        byCustomer.Should().Contain(t => t.Id == created.Id);

        var detail = await _client.GetFromJsonAsync<TicketDetailDto>($"/api/tickets/{created.Id}");
        detail!.CustomerName.Should().Be("Acme Industries");
    }

    [Fact]
    [Trait("Story", "CRM-005")]
    public async Task Classification_required_and_update_works()
    {
        var bad = await _client.PostAsJsonAsync("/api/tickets", new CreateTicketRequest(
            Guid.NewGuid().ToString(),
            "X",
            "No category",
            null,
            "",
            "High"));
        bad.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var created = await CreateAsync("Beta", "Classify me");
        var update = await _client.PutAsJsonAsync($"/api/tickets/{created.Id}/classification",
            new UpdateClassificationRequest("Technical", "Urgent"));
        update.EnsureSuccessStatusCode();
        var summary = await update.Content.ReadFromJsonAsync<TicketSummaryDto>();
        summary!.Priority.Should().Be("Urgent");
        summary.Category.Should().Be("Technical");
    }

    [Fact]
    [Trait("Story", "CRM-006")]
    public async Task Assign_reassign_and_filter_assigned()
    {
        var created = await CreateAsync("Acme", "Assign me");
        var agentId = "11111111-1111-1111-1111-111111111111";
        var assign = await _client.PostAsJsonAsync($"/api/tickets/{created.Id}/assign",
            new AssignTicketRequest(agentId, "Demo Agent"));
        assign.EnsureSuccessStatusCode();

        var mine = await _client.GetFromJsonAsync<List<TicketSummaryDto>>($"/api/tickets?assignedTo={agentId}");
        mine.Should().Contain(t => t.Id == created.Id);

        var reassign = await _client.PostAsJsonAsync($"/api/tickets/{created.Id}/assign",
            new AssignTicketRequest("22222222-2222-2222-2222-222222222222", "Lead Agent"));
        reassign.EnsureSuccessStatusCode();

        var detail = await _client.GetFromJsonAsync<TicketDetailDto>($"/api/tickets/{created.Id}");
        detail!.AssignedAgentName.Should().Be("Lead Agent");
        detail.History.Should().Contain(h => h.Field == "AssignedAgent");
    }

    [Fact]
    [Trait("Story", "CRM-007")]
    public async Task Status_transition_escalate_and_history()
    {
        var created = await CreateAsync("Acme", "Lifecycle");
        var bad = await _client.PostAsJsonAsync($"/api/tickets/{created.Id}/status", new ChangeStatusRequest("Resolved"));
        bad.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var ok = await _client.PostAsJsonAsync($"/api/tickets/{created.Id}/status", new ChangeStatusRequest("InProgress"));
        ok.EnsureSuccessStatusCode();

        var escalate = await _client.PostAsJsonAsync($"/api/tickets/{created.Id}/escalate",
            new EscalateTicketRequest("22222222-2222-2222-2222-222222222222", "Lead Agent"));
        escalate.EnsureSuccessStatusCode();

        var detail = await _client.GetFromJsonAsync<TicketDetailDto>($"/api/tickets/{created.Id}");
        detail!.IsEscalated.Should().BeTrue();
        detail.Status.Should().Be("InProgress");
        detail.History.Should().Contain(h => h.Field == "Status");
        detail.History.Should().Contain(h => h.Field == "Escalated");
    }

    [Fact]
    [Trait("Story", "CRM-014")]
    public async Task Ticket_task_create_complete_and_list_my_open()
    {
        var created = await CreateAsync("Task Co", "Need follow-up call");
        var agentId = "11111111-1111-1111-1111-111111111111";
        var due = DateTimeOffset.UtcNow.Date.AddHours(18);

        var empty = await _client.PostAsJsonAsync($"/api/tickets/{created.Id}/tasks",
            new CreateTicketTaskRequest("  ", due, agentId, "Agent"));
        empty.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var createdTask = await _client.PostAsJsonAsync($"/api/tickets/{created.Id}/tasks",
            new CreateTicketTaskRequest("Call customer AP", due, agentId, "Agent"));
        createdTask.EnsureSuccessStatusCode();
        var task = await createdTask.Content.ReadFromJsonAsync<TicketTaskDto>();
        task!.Title.Should().Be("Call customer AP");
        task.Status.Should().Be("Open");

        var onTicket = await _client.GetFromJsonAsync<List<TicketTaskDto>>($"/api/tickets/{created.Id}/tasks");
        onTicket.Should().Contain(t => t.Id == task.Id);

        var mine = await _client.GetFromJsonAsync<List<TicketTaskDto>>(
            $"/api/tickets/tasks?assignedTo={agentId}&dueBefore={Uri.EscapeDataString(due.AddDays(1).ToString("O"))}");
        mine.Should().Contain(t => t.Id == task.Id);

        var complete = await _client.PostAsync($"/api/tickets/{created.Id}/tasks/{task.Id}/complete", null);
        complete.EnsureSuccessStatusCode();
        var done = await complete.Content.ReadFromJsonAsync<TicketTaskDto>();
        done!.Status.Should().Be("Completed");

        var mineAfter = await _client.GetFromJsonAsync<List<TicketTaskDto>>(
            $"/api/tickets/tasks?assignedTo={agentId}");
        mineAfter.Should().NotContain(t => t.Id == task.Id);
    }

    [Fact]
    [Trait("Story", "CRM-015")]
    public async Task Quick_replies_catalog_returns_shared_seed()
    {
        var replies = await _client.GetFromJsonAsync<List<QuickReplyDto>>("/api/tickets/quick-replies");
        replies.Should().NotBeNull();
        replies!.Count.Should().BeGreaterThanOrEqualTo(3);
        replies.Should().Contain(r => r.Title.Contains("Billing", StringComparison.OrdinalIgnoreCase));
        replies.Should().OnlyContain(r => !string.IsNullOrWhiteSpace(r.Body));
    }

    [Fact]
    [Trait("Story", "CRM-016")]
    public async Task Internal_note_with_mention_persists_on_detail()
    {
        var created = await CreateAsync("Acme", "Need collaborator");
        var empty = await _client.PostAsJsonAsync($"/api/tickets/{created.Id}/notes",
            new AddTicketNoteRequest("   "));
        empty.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        using var req = new HttpRequestMessage(HttpMethod.Post, $"/api/tickets/{created.Id}/notes");
        req.Headers.Add("X-Crm-User-Email", "agent@crm.local");
        req.Headers.Add("X-Crm-User-Id", "11111111-1111-1111-1111-111111111111");
        req.Content = JsonContent.Create(new AddTicketNoteRequest(
            "Please review billing lines @Lead Agent"));
        var added = await _client.SendAsync(req);
        added.EnsureSuccessStatusCode();
        var note = await added.Content.ReadFromJsonAsync<TicketNoteDto>();
        note!.Body.Should().Contain("Lead Agent");
        note.MentionedUserIds.Should().Contain("22222222-2222-2222-2222-222222222222");

        var detail = await _client.GetFromJsonAsync<TicketDetailDto>($"/api/tickets/{created.Id}");
        detail!.Notes.Should().Contain(n => n.Id == note.Id);
        detail.Notes[0].AuthorName.Should().Be("agent@crm.local");
    }

    private async Task<TicketSummaryDto> CreateAsync(string customerName, string subject)
    {
        var response = await _client.PostAsJsonAsync("/api/tickets", new CreateTicketRequest(
            Guid.NewGuid().ToString(),
            customerName,
            subject,
            "details",
            "General",
            "Medium"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TicketSummaryDto>())!;
    }
}

public sealed class TicketsApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dataPath = Path.Combine(Path.GetTempPath(), "crm-tickets-tests", Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_dataPath);
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Tickets:DataPath"] = _dataPath,
                ["Tickets:Provider"] = "Sqlite",
                ["ConnectionStrings:Tickets"] = string.Empty
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
