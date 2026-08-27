using Crm.BuildingBlocks.Endpoints;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ListErpDeliveries;

/// <summary>SDD CRM-039 polish / specs/044</summary>
public sealed class ListErpDeliveriesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tickets/integrations/erp-deliveries", async (int? take, IMediator mediator) =>
            Results.Ok(await mediator.Send(new ListErpDeliveriesQuery(take ?? 20))));
    }
}

public sealed record ListErpDeliveriesQuery(int Take) : IRequest<IReadOnlyList<ErpDeliveryRecord>>;

public sealed class ListErpDeliveriesHandler(ErpWebhookNotifier erp)
    : IRequestHandler<ListErpDeliveriesQuery, IReadOnlyList<ErpDeliveryRecord>>
{
    public Task<IReadOnlyList<ErpDeliveryRecord>> Handle(
        ListErpDeliveriesQuery request,
        CancellationToken cancellationToken)
        => Task.FromResult(erp.RecentDeliveries(request.Take));
}
