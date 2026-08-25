using System.Text.Json;
using Crm.Tickets.Api.Domain;
using Crm.Tickets.Api.Infrastructure;
using FluentAssertions;
using Xunit;

namespace Crm.Tickets.Api.Tests;

/// <summary>SDD CRM-039</summary>
public sealed class ErpWebhookTests
{
    [Fact]
    [Trait("Story", "CRM-039")]
    public void BuildPayload_includes_ticket_fields()
    {
        var ticket = Ticket.Create(
            "TKT-9001",
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "Acme",
            "ERP sync subject",
            "desc",
            "Billing",
            "Medium",
            "test");
        var payload = ErpWebhookNotifier.BuildPayload(ticket);
        var json = JsonSerializer.Serialize(payload);
        json.Should().Contain("TKT-9001");
        json.Should().Contain("ERP sync subject");
        json.Should().Contain("ticketId");
        json.Should().Contain("customerId");
        json.Should().Contain("status");
    }
}
