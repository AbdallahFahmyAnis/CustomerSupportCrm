using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Features.Shared;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.RunAutomation;

public sealed class RunAutomationHandler(TicketsDb db, SlaAutomationClient sla)
    : IRequestHandler<RunAutomationCommand, RunAutomationResponse>
{
    public async Task<RunAutomationResponse> Handle(RunAutomationCommand request, CancellationToken cancellationToken)
    {
        var ticket = db.Get(request.Id);
        if (ticket is null)
        {
            return new RunAutomationResponse(null, "Ticket not found.");
        }

        var assigned = false;
        var escalated = false;
        var notes = new List<string>();

        if (string.IsNullOrWhiteSpace(ticket.AssignedAgentId))
        {
            var suggestion = await sla.SuggestAssigneeAsync(ticket.Category, ticket.Priority, cancellationToken);
            if (suggestion?.AgentId is not null && suggestion.AgentName is not null)
            {
                ticket.Assign(suggestion.AgentId, suggestion.AgentName, "SLA automation");
                assigned = true;
                notes.Add($"Assigned to {suggestion.AgentName}");
            }
        }

        var decision = await sla.ShouldEscalateAsync(
            ticket.Priority,
            ticket.CreatedAt,
            ticket.IsEscalated,
            ticket.Status,
            ticket.AssignedAgentId,
            cancellationToken);
        if (decision is { ShouldEscalate: true } && !ticket.IsEscalated)
        {
            ticket.Escalate(decision.AssignToAgentId, decision.AssignToAgentName, "SLA automation");
            escalated = true;
            notes.Add(decision.Reason ?? "Escalated");
        }

        if (assigned || escalated)
        {
            db.Update(ticket);
        }

        var message = notes.Count == 0 ? "No automation changes." : string.Join("; ", notes);
        return new RunAutomationResponse(
            new RunAutomationResultDto(TicketMap.Summary(ticket), assigned, escalated, message),
            null);
    }
}
