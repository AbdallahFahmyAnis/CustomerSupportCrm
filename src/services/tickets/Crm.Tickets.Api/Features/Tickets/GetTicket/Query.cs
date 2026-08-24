using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetTicket;

/// <summary>SDD CRM-004 / specs/003-ticket-lifecycle.</summary>
public sealed record GetTicketQuery(Guid Id) : IRequest<TicketDetailDto?>;
