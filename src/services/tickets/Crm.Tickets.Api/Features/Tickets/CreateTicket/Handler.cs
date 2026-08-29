using Crm.BuildingBlocks.Audit;
using Crm.Tickets.Api.Domain;
using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Crm.Tickets.Api.Features.Tickets.CreateTicket;

/// <summary>SDD CRM-004 / CRM-018 / CRM-039 / CRM-036 / specs/051.</summary>
public sealed class CreateTicketHandler(
    TicketsDb db,
    SlaAutomationClient sla,
    ErpWebhookNotifier erp,
    IdentityAuditClient audit,
    IHttpContextAccessor http)
    : IRequestHandler<CreateTicketCommand, CreateTicketResponse>
{
    public async Task<CreateTicketResponse> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var ticket = Ticket.Create(
                db.NextTicketNumber(),
                request.CustomerId,
                request.CustomerName,
                request.Subject,
                request.Description,
                request.Category,
                request.Priority,
                request.Actor);
            if (request.DepartmentId is { } dept)
            {
                ticket.SetDepartment(dept);
            }

            db.Insert(ticket);

            var suggestion = await sla.SuggestAssigneeAsync(ticket.Category, ticket.Priority, cancellationToken);
            if (suggestion?.AgentId is not null && suggestion.AgentName is not null)
            {
                ticket.Assign(suggestion.AgentId, suggestion.AgentName, "SLA automation");
                db.Update(ticket);
            }

            await erp.NotifyTicketCreatedAsync(ticket, cancellationToken);
            await audit.WriteAsync(
                AuditServices.Tickets,
                "TicketCreated",
                true,
                AuditActor.Email(http) ?? request.Actor,
                ticket.TicketNumber,
                ticket.Subject,
                cancellationToken);

            return new CreateTicketResponse(TicketMap.Summary(ticket), null);
        }
        catch (ArgumentException ex)
        {
            return new CreateTicketResponse(null, ex.Message);
        }
    }
}
