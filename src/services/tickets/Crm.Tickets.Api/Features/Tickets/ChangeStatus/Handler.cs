using Crm.BuildingBlocks.Audit;
using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Crm.Tickets.Api.Features.Tickets.ChangeStatus;

/// <summary>SDD CRM-007 / CRM-036 / specs/051.</summary>
public sealed class ChangeStatusHandler(
    TicketsDb db,
    IdentityAuditClient audit,
    IHttpContextAccessor http) : IRequestHandler<ChangeStatusCommand, ChangeStatusResponse>
{
    public async Task<ChangeStatusResponse> Handle(ChangeStatusCommand request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return new ChangeStatusResponse(null, "Ticket not found.");
        }

        try
        {
            ticket.ChangeStatus(request.Status, request.Actor);
            db.Update(ticket);
            await audit.WriteAsync(
                AuditServices.Tickets,
                "TicketStatusChanged",
                true,
                AuditActor.Email(http) ?? request.Actor,
                ticket.TicketNumber,
                $"Status → {request.Status}",
                cancellationToken);
            return new ChangeStatusResponse(TicketMap.Summary(ticket), null);
        }
        catch (InvalidOperationException ex)
        {
            return new ChangeStatusResponse(null, ex.Message);
        }
    }
}
