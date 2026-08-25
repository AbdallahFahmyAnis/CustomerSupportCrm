using Crm.BuildingBlocks.Endpoints;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Settings.GetErpWebhook;

/// <summary>SDD CRM-039 / 048 — tickets service reads URL + auth (no admin cookie).</summary>
public sealed class GetErpWebhookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/identity/integrations/erp", async (IMediator mediator) =>
            Results.Ok(await mediator.Send(new GetErpWebhookQuery())));
    }
}

public sealed record GetErpWebhookQuery : IRequest<ErpWebhookDto>;
public sealed record ErpWebhookDto(string WebhookUrl, string AuthHeader = "");

public sealed class GetErpWebhookHandler(IdentityDirectory directory)
    : IRequestHandler<GetErpWebhookQuery, ErpWebhookDto>
{
    public async Task<ErpWebhookDto> Handle(GetErpWebhookQuery request, CancellationToken cancellationToken)
    {
        var row = await directory.GetOrCreateSettingsAsync(cancellationToken);
        return new ErpWebhookDto(row.ErpWebhookUrl ?? "", row.ErpWebhookAuthHeader ?? "");
    }
}
