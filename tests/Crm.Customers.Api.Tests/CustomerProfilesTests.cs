using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Crm.Contracts.Customers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Crm.Customers.Api.Tests;

public sealed class CustomerProfilesTests : IClassFixture<CustomerApiFactory>
{
    private readonly HttpClient _client;

    public CustomerProfilesTests(CustomerApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    [Trait("Story", "CRM-001")]
    public async Task Create_search_and_update_customer_profile()
    {
        var create = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerRequest(
            "Gamma Co",
            $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            "Gamma",
            "Active"));
        create.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await create.Content.ReadFromJsonAsync<CustomerSummaryDto>();
        created.Should().NotBeNull();

        var search = await _client.GetFromJsonAsync<List<CustomerSummaryDto>>($"/api/customers?q={Uri.EscapeDataString(created!.DisplayName)}");
        search.Should().Contain(c => c.Id == created.Id);

        var update = await _client.PutAsJsonAsync($"/api/customers/{created.Id}", new UpdateCustomerRequest(
            "Gamma Co Updated",
            created.UniqueIdentifier,
            "Gamma Org",
            "Active"));
        update.StatusCode.Should().Be(HttpStatusCode.OK);
        var detail = await _client.GetFromJsonAsync<CustomerDetailDto>($"/api/customers/{created.Id}");
        detail!.DisplayName.Should().Be("Gamma Co Updated");
        detail.Organization.Should().Be("Gamma Org");
    }

    [Fact]
    [Trait("Story", "CRM-001")]
    public async Task Duplicate_unique_identifier_returns_conflict()
    {
        var uid = $"DUP-{Guid.NewGuid():N}".Substring(0, 12);
        var first = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerRequest("One", uid, null, null));
        first.EnsureSuccessStatusCode();
        var second = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerRequest("Two", uid, null, null));
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var warning = await second.Content.ReadFromJsonAsync<DuplicateWarningDto>();
        warning!.Message.Should().Contain("already exists");
    }

    [Fact]
    [Trait("Story", "CRM-002")]
    public async Task Add_and_deactivate_contact()
    {
        var created = await CreateCustomerAsync("Contact Co");
        var add = await _client.PostAsJsonAsync($"/api/customers/{created.Id}/contacts", new AddContactRequest("email", "a@example.com", true));
        add.StatusCode.Should().Be(HttpStatusCode.Created);
        var contact = await add.Content.ReadFromJsonAsync<ContactDto>();
        contact!.IsActive.Should().BeTrue();

        var deactivate = await _client.PostAsync($"/api/customers/{created.Id}/contacts/{contact.Id}/deactivate", null);
        deactivate.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var detail = await _client.GetFromJsonAsync<CustomerDetailDto>($"/api/customers/{created.Id}");
        detail!.Contacts.Single(c => c.Id == contact.Id).IsActive.Should().BeFalse();
    }

    [Fact]
    [Trait("Story", "CRM-003")]
    public async Task Add_note_and_attachment_appear_on_timeline()
    {
        var created = await CreateCustomerAsync("Timeline Co");
        var note = await _client.PostAsJsonAsync($"/api/customers/{created.Id}/notes", new AddNoteRequest("Called about renewal."));
        note.StatusCode.Should().Be(HttpStatusCode.Created);

        using var content = new MultipartFormDataContent();
        var bytes = new ByteArrayContent("hello"u8.ToArray());
        bytes.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        content.Add(bytes, "file", "hello.txt");
        var upload = await _client.PostAsync($"/api/customers/{created.Id}/attachments", content);
        upload.StatusCode.Should().Be(HttpStatusCode.Created);
        var attachment = await upload.Content.ReadFromJsonAsync<AttachmentDto>();

        var detail = await _client.GetFromJsonAsync<CustomerDetailDto>($"/api/customers/{created.Id}");
        detail!.Timeline.Should().Contain(t => t.Kind == "note");
        detail.Timeline.Should().Contain(t => t.Kind == "attachment" && t.Summary == "hello.txt");

        var download = await _client.GetAsync($"/api/customers/{created.Id}/attachments/{attachment!.Id}");
        download.StatusCode.Should().Be(HttpStatusCode.OK);
        (await download.Content.ReadAsStringAsync()).Should().Be("hello");
    }

    private async Task<CustomerSummaryDto> CreateCustomerAsync(string name)
    {
        var response = await _client.PostAsJsonAsync("/api/customers", new CreateCustomerRequest(
            name,
            $"CUST-{Guid.NewGuid():N}".Substring(0, 12),
            null,
            "Active"));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<CustomerSummaryDto>())!;
    }
}

public sealed class CustomerApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dataPath = Path.Combine(Path.GetTempPath(), "crm-customers-tests", Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_dataPath);
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Customers:DataPath"] = _dataPath,
                ["Customers:Provider"] = "Sqlite",
                ["ConnectionStrings:Customers"] = string.Empty
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
            // best effort cleanup
        }
    }
}
