using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetTicketOptions;

/// <summary>SDD CRM-005 / specs/003-ticket-lifecycle.</summary>
public sealed record GetTicketOptionsQuery : IRequest<TicketOptionsDto>;
