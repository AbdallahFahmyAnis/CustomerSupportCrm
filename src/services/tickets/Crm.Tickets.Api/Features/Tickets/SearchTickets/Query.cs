using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.SearchTickets;

/// <summary>SDD CRM-004 / CRM-043.</summary>
public sealed record SearchTicketsQuery(string? Q, string? AssignedAgentId, string? DepartmentId = null)
    : IRequest<IReadOnlyList<TicketSummaryDto>>;
