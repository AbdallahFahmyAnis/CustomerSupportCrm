using System.Net;
using System.Net.Http;
using Crm.Tickets.Api.Domain;
using Crm.Tickets.Api.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Crm.Tickets.Api.Tests;

/// <summary>SDD CRM-039 polish / specs/044</summary>
public sealed class ErpWebhookRetryTests
{
    [Fact]
    [Trait("Story", "CRM-039")]
    public void BuildPayload_includes_ticket_fields()
    {
        var ticket = SampleTicket();
        var payload = ErpWebhookNotifier.BuildPayload(ticket);
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        json.Should().Contain("TKT-9001");
        json.Should().Contain("ERP sync subject");
        json.Should().Contain("ticketId");
        json.Should().Contain("customerId");
        json.Should().Contain("status");
    }

    [Fact]
    [Trait("Story", "CRM-039")]
    public async Task Empty_url_is_noop_and_leaves_log_empty()
    {
        var handler = new CountingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var notifier = CreateNotifier(handler);
        await notifier.DeliverToUrlAsync("  ", SampleTicket());
        handler.Calls.Should().Be(0);
        notifier.RecentDeliveries().Should().BeEmpty();
    }

    [Fact]
    [Trait("Story", "CRM-039")]
    public async Task Retries_on_non_2xx_then_logs_failure()
    {
        var handler = new CountingHandler(_ => new HttpResponseMessage(HttpStatusCode.BadGateway));
        var notifier = CreateNotifier(handler);
        notifier.RetryDelay = TimeSpan.Zero;
        await notifier.DeliverToUrlAsync("http://erp.test/hook", SampleTicket());
        handler.Calls.Should().Be(3);
        notifier.RecentDeliveries().Should().ContainSingle(d => d.Status == "fail:502");
    }

    [Fact]
    [Trait("Story", "CRM-039")]
    public async Task Succeeds_on_retry_and_logs_ok()
    {
        var n = 0;
        var handler = new CountingHandler(_ =>
        {
            n++;
            return new HttpResponseMessage(n < 2 ? HttpStatusCode.ServiceUnavailable : HttpStatusCode.OK);
        });
        var notifier = CreateNotifier(handler);
        notifier.RetryDelay = TimeSpan.Zero;
        await notifier.DeliverToUrlAsync("http://erp.test/hook", SampleTicket());
        handler.Calls.Should().Be(2);
        notifier.RecentDeliveries().Should().ContainSingle(d => d.Status.StartsWith("ok:"));
    }

    [Fact]
    [Trait("Story", "CRM-039")]
    public async Task Sends_authorization_header_when_configured()
    {
        string? auth = null;
        var handler = new CountingHandler(req =>
        {
            auth = req.Headers.Authorization?.ToString()
                   ?? (req.Headers.TryGetValues("Authorization", out var v) ? string.Join(",", v) : null);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        var notifier = CreateNotifier(handler);
        await notifier.DeliverToUrlAsync("http://erp.test/hook", SampleTicket(), "Bearer secret-token");
        handler.Calls.Should().Be(1);
        auth.Should().Contain("Bearer secret-token");
        notifier.RecentDeliveries().Should().ContainSingle(d => d.Status.StartsWith("ok:"));
    }

    private static Ticket SampleTicket() => Ticket.Create(
        "TKT-9001",
        Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
        "Acme",
        "ERP sync subject",
        "desc",
        "Billing",
        "Medium",
        "test");

    private static ErpWebhookNotifier CreateNotifier(HttpMessageHandler handler)
    {
        var factory = new FixedHttpClientFactory(handler);
        var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
        return new ErpWebhookNotifier(factory, config, NullLogger<ErpWebhookNotifier>.Instance);
    }

    private sealed class FixedHttpClientFactory(HttpMessageHandler handler) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => new(handler, disposeHandler: false);
    }

    private sealed class CountingHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(respond(request));
        }
    }
}
